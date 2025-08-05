using UnityEngine;

public class LevelScoreResult
{
    public int BaseScore { get; private set; }
    public int TimeBonus { get; private set; }
    public int TotalScore => BaseScore + TimeBonus;

    public LevelScoreResult(int baseScore, int timeBonus)
    {
        BaseScore = baseScore;
        TimeBonus = timeBonus;
    }
}

public class ScoreManager : Singleton<ScoreManager>
{
    int curScore = 0;
    public void OnInit(int score)
    {
        curScore = score;
        UIManager.Instance.GetUI<CanvasGamePlay>().UpdateScore(curScore);
    }
    public void AddScore(int amount)
    {
        if (GameManager.IsState(GameState.GamePlay))
        {
            curScore += amount;
            UIManager.Instance.GetUI<CanvasGamePlay>().UpdateScore(curScore);
        }
    }
    public LevelScoreResult CalScoreOnWin(float timeRemaining)
    {
        int timeBonus = Mathf.RoundToInt(timeRemaining);
        LevelScoreResult result = new LevelScoreResult(curScore, timeBonus);
        DataManager.Instance.SaveWinningScore(result.TotalScore);

        return result;
    }
    public LevelScoreResult CalScoreOnLose()
    {
        LevelScoreResult result = new LevelScoreResult(curScore, 0);
        DataManager.Instance.ResetTotalScore();
        return result;
    }
}
