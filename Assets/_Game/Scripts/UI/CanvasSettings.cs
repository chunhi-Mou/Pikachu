using System.Collections;
using UnityEngine;

public enum SettingsContext
{
    FromMainMenu,
    FromGameplay
}
public class CanvasSettings : UICanvas
{
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject gameplayButtons;
    [SerializeField] private Animator settingsUIAnimator;
    private SettingsContext currSettingsContext;
    private string currSettingsUIAnim;
    public void SetState(SettingsContext context)
    {
        SoundManager.Instance.PlayFx(FxID.SwipeOn);
        GameManager.ChangeState(GameState.Setting);
        AnimatorUtils.ChangeAnim(GameCONST.SETTINGS_ON, settingsUIAnimator, ref currSettingsUIAnim);
        currSettingsContext = context;
        DisplayButtons(currSettingsContext);
    }

    private void DisplayButtons(SettingsContext context)
    {
        switch (context)
        {
            case SettingsContext.FromMainMenu:
                mainMenuButtons.gameObject.SetActive(true);
                gameplayButtons.gameObject.SetActive(false);
                break;
            case SettingsContext.FromGameplay:
                mainMenuButtons.gameObject.SetActive(false);
                gameplayButtons.gameObject.SetActive(true);
                break;
        }
    }
    public void ContinueButton(float timer)
    {
        StartCoroutine(ContinueGamePlay(0.2f, timer, currSettingsContext));
    }
    private IEnumerator ContinueGamePlay(float delay, float timer, SettingsContext context)
    {
        SoundManager.Instance.PlayFx(FxID.SwipeOff);
        AnimatorUtils.ChangeAnim(GameCONST.SETTINGS_OFF, settingsUIAnimator, ref currSettingsUIAnim);
        switch (context)
        {
            case SettingsContext.FromMainMenu:
                mainMenuButtons.gameObject.SetActive(false);
                break;
            case SettingsContext.FromGameplay:
                gameplayButtons.gameObject.SetActive(false);
                GameManager.ChangeState(GameState.GamePlay);
                break;
        }
        yield return new WaitForSeconds(delay);
        UIManager.Instance.CloseUI<CanvasSettings>(timer);
    }
    public void MainMenuButton()
    {
        SoundManager.Instance.PlayFx(FxID.Button);
        UIManager.Instance.CloseAllUI();
        GameManager.ChangeState(GameState.MainMenu);
        UIManager.Instance.OpenUI<CanvasMainMenu>();
    }
}
