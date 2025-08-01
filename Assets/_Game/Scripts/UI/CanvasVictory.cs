using TMPro;
using UnityEngine;

public class CanvasVictory : UICanvas
{
	[SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI highestScoreText;
    [SerializeField] TextMeshProUGUI totalLvScoreText;
    [SerializeField] TextMeshProUGUI currentScoreText;
    public void SetBaseInfo(LevelScoreResult result)
    {
        LevelManager.Instance.OnNextLevel();

        scoreText.text = result.BaseScore.ToString();
        timerText.text = result.TimeBonus.ToString();
        totalLvScoreText.text = result.TotalScore.ToString();
        highestScoreText.text = DataManager.Instance.GetHighestScore().ToString();
        currentScoreText.text = DataManager.Instance.GetCurrentScore().ToString();
    }
	    public void MainMenuButton()
    {
        SoundManager.Instance.PlayFx(FxID.Button);
        Close(0);
        UIManager.Instance.OpenUI<CanvasMainMenu>();
    }
    public void NextLevelButton()
    {
        SoundManager.Instance.PlayFx(FxID.Button);
        GameManager.Instance.StartGame();
    }
}
