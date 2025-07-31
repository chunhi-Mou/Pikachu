using TMPro;
using UnityEngine;

public class CanvasMainMenu : UICanvas
{
    [SerializeField] TextMeshProUGUI highestScoreText;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    private void Awake()
    {
        Setup();
    }

    public override void Setup()
    {
        currentLevelText.text = "Level: " + DataManager.Instance.GetCurLevel();
        highestScoreText.text = DataManager.Instance.GetHighestScore().ToString();
    }
    public void PlayButton()
    {
        GameManager.Instance.StartGame();
    }

    public void SettingsButton()
    {
        UIManager.Instance.OpenUI<CanvasSettings>().SetState(this);
        UIManager.Instance.CloseUI<CanvasGamePlay>(0);
    }
}
