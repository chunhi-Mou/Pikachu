using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public int GetHighestLevel()
    {
        return PlayerPrefs.GetInt(GameCONST.HIGHEST_PLAYED_LEVEL, 1);
    }

    public int GetHighestScore()
    {
        return PlayerPrefs.GetInt(GameCONST.HIGHEST_SCORE, 0);
    }
    public int GetTotalScore()
    {
        return PlayerPrefs.GetInt(GameCONST.SCORE, 0);
    }
    public void SaveTotalScore(int currentScore)
    {
        int totalScore = PlayerPrefs.GetInt(GameCONST.SCORE, 0) + currentScore;
        PlayerPrefs.SetInt(GameCONST.SCORE, totalScore);
        //SAVE HIGHEST SCORE
        if (totalScore > PlayerPrefs.GetInt(GameCONST.HIGHEST_SCORE, 0))
        {
            PlayerPrefs.SetInt(GameCONST.HIGHEST_SCORE, totalScore);
        }
        PlayerPrefs.Save();
    }
    public void UpdatePlayedLevel(int level)
    {
        int saveLevel = PlayerPrefs.GetInt(GameCONST.HIGHEST_PLAYED_LEVEL, level);
        if (level >= saveLevel)
        {
            PlayerPrefs.SetInt(GameCONST.HIGHEST_PLAYED_LEVEL, level);
            PlayerPrefs.Save();
        }
    }
}
