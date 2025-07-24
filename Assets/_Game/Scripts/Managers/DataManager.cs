using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public int GetCurLevel()
    {
        return PlayerPrefs.GetInt(GameCONST.CURRENT_LEVEL, 1);
    }

    public int GetHighestScore()
    {
        return PlayerPrefs.GetInt(GameCONST.HIGHEST_SCORE, 0);
    }

    public int GetCurrentScore()
    {
        return PlayerPrefs.GetInt(GameCONST.SCORE, 0);
    }
    public void SaveWinningScore(int totalLevelScore)
    {
        int scoreThisLevel = totalLevelScore;
        int newTotalScore = PlayerPrefs.GetInt(GameCONST.SCORE, 0) + scoreThisLevel;
        PlayerPrefs.SetInt(GameCONST.SCORE, newTotalScore);
        if (newTotalScore > GetHighestScore())
        {
            PlayerPrefs.SetInt(GameCONST.HIGHEST_SCORE, newTotalScore);
        }
        PlayerPrefs.Save();
    }
    public void ResetTotalScore()
    {
        PlayerPrefs.SetInt(GameCONST.SCORE, 0);
        PlayerPrefs.Save();
    }
    public void UpdatePlayedLevel(int level)
    {
        int saveLevel = PlayerPrefs.GetInt(GameCONST.CURRENT_LEVEL, level);
        PlayerPrefs.SetInt(GameCONST.CURRENT_LEVEL, level);
        PlayerPrefs.Save();
    }
}
