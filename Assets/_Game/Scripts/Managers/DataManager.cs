using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public int GetCurLevel()
    {
        return PlayerPrefs.GetInt(GameCONST.Data_CURRENT_LEVEL, 1);
    }

    public int GetHighestScore()
    {
        return PlayerPrefs.GetInt(GameCONST.Data_HIGHEST_SCORE, 0);
    }

    public int GetCurrentScore()
    {
        return PlayerPrefs.GetInt(GameCONST.Data_SCORE, 0);
    }
    public void SaveWinningScore(int totalLevelScore)
    {
        int scoreThisLevel = totalLevelScore;
        int newTotalScore = PlayerPrefs.GetInt(GameCONST.Data_SCORE, 0) + scoreThisLevel;
        PlayerPrefs.SetInt(GameCONST.Data_SCORE, newTotalScore);
        if (newTotalScore > GetHighestScore())
        {
            PlayerPrefs.SetInt(GameCONST.Data_HIGHEST_SCORE, newTotalScore);
        }
        PlayerPrefs.Save();
    }
    public void ResetTotalScore()
    {
        PlayerPrefs.SetInt(GameCONST.Data_SCORE, 0);
        PlayerPrefs.Save();
    }
    public void UpdatePlayedLevel(int level)
    {
        PlayerPrefs.SetInt(GameCONST.Data_CURRENT_LEVEL, level);
        PlayerPrefs.Save();
    }
    public bool GetSoundState(string volumeKey)
    {
        return PlayerPrefs.GetInt(volumeKey, 1) == 1;
    }
    public void SaveSoundState(string volumeKey, bool isOn)
    {
        PlayerPrefs.SetInt(volumeKey, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
