using TMPro;
using UnityEngine;

public class CanvasFail : UICanvas
{
    [SerializeField] TextMeshProUGUI scoreText;

    public void SetBaseScore(LevelScoreResult result)
    {
        scoreText.text = result.BaseScore.ToString();
    }
    public void MainMenuButton()
    {
        SoundManager.Instance.PlayFx(FxID.Button);
        Close(0);
        UIManager.Instance.OpenUI<CanvasMainMenu>();
    }
        
    public void ReplayButton()
    {
        SoundManager.Instance.PlayFx(FxID.Button);
        GameManager.Instance.StartGame();
    }
}
