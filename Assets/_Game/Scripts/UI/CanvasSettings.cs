using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CanvasSettings : UICanvas
{
    //[SerializeField] private GameObject[] buttons;
    [SerializeField] private Animator settingsUIAnimator;
    public void SetState(UICanvas canvas)
    {
        GameManager.ChangeState(GameState.Setting);
        settingsUIAnimator.SetTrigger(GameCONST.SETTINGS_ON);
        /*for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
        
        if (canvas is CanvasMainMenu)
        {
            buttons[2].gameObject.SetActive(true); // -> Not opt yet
        } else if (canvas is CanvasGamePlay)
        {
            buttons[0].gameObject.SetActive(true);
            buttons[1].gameObject.SetActive(true);
        }*/
    }

    public void ContinueButton(float timer)
    {
        StartCoroutine(ContinueGamePlay(0.2f, timer));
    }

    private IEnumerator ContinueGamePlay(float delay, float timer)
    {
        settingsUIAnimator.SetTrigger(GameCONST.SETTINGS_OFF);
        yield return new WaitForSeconds(delay);
        GameManager.ChangeState(GameState.GamePlay);
        UIManager.Instance.CloseUI<CanvasSettings>(timer);
    }
    public void MainMenuButton()
    {
        UIManager.Instance.CloseAllUI();
        GameManager.ChangeState(GameState.MainMenu);
        UIManager.Instance.OpenUI<CanvasMainMenu>();
    }
}
