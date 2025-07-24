using TMPro;
using UnityEngine;

public class CanvasVictory : UICanvas
{
	[SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI highestScoreText;
    [SerializeField] TextMeshProUGUI totalLvScoreText;
    [SerializeField] TextMeshProUGUI currentScoreText;
    public void SetBaseInfo(int baseScore, int timeBonus, int totalLevelScore)
    {
        scoreText.text = baseScore.ToString();
        timerText.text = timeBonus.ToString();
        highestScoreText.text = DataManager.Instance.GetHighestScore().ToString();
        totalLvScoreText.text = totalLevelScore.ToString();
        currentScoreText.text = DataManager.Instance.GetCurrentScore().ToString();
    }
	    public void MainMenuButton()
    {
        LevelManager.Instance.OnNextLevel(false);
        Close(0);
        UIManager.Instance.OpenUI<CanvasMainMenu>();
    }
    public void NextLevelButton()
    {
        LevelManager.Instance.OnNextLevel(true);
    }
}
