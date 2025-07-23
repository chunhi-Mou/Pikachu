using TMPro;
using UnityEngine;

public class CanvasVictory : UICanvas
{
	[SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timerText;

    public void SetBaseInfo(int baseScore, string baseTimer)
    {
        scoreText.text = baseScore.ToString();
        timerText.text = baseTimer;
    }
	    public void MainMenuButton()
    {
        Close(0);
        UIManager.Instance.OpenUI<CanvasMainMenu>();
    }

    public void NextLevelButton()
    {
        LevelManager.Instance.OnNextLevel();
    }
}
