using TMPro;
using UnityEngine;

public class CanvasFail : UICanvas
{
    [SerializeField] TextMeshProUGUI scoreText;

    public void SetBaseScore(LevelScoreResult result)
    {
        SoundManager.Instance.PauseMusic();
        SoundManager.Instance.StopAllFx();
        SoundManager.Instance.PlayFx(FxID.Lose);
        scoreText.text = result.BaseScore.ToString();
    }
    public void MainMenuButton()
    {
        SoundManager.Instance.PlayFx(FxID.Button);
        SoundManager.Instance.ResumeMusic();
        Close(0);
        UIManager.Instance.OpenUI<CanvasMainMenu>();
    }
        
    public void ReplayButton()
    {
        SoundManager.Instance.PlayFx(FxID.Button);
        SoundManager.Instance.ResumeMusic();
        GameManager.Instance.StartGame();
    }
}
