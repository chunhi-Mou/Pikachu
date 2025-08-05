using System;
using System.Collections;
using UnityEngine;

public enum GameState { MainMenu, GamePlay, Finish, Revive, Setting, Pause }

public class GameManager : Singleton<GameManager>
{
    [SerializeField] Animator transitionAnimator;
    private static GameState gameState;

    public static void ChangeState(GameState state)
    {
        gameState = state;
    }
    
    public static bool IsState(GameState state) => gameState == state;
    
    private void Awake()
    {
        JsonUtils.OnInit();
        //tranh viec nguoi choi cham da diem vao man hinh
        Input.multiTouchEnabled = false;
        //target frame rate ve 60 fps
        Application.targetFrameRate = 60;
        //tranh viec tat man hinh
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //xu tai tho
        int maxScreenHeight = 1280;
        float ratio = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
        if (Screen.currentResolution.height > maxScreenHeight)
        {
            Screen.SetResolution(Mathf.RoundToInt(ratio * (float)maxScreenHeight), maxScreenHeight, true);
        }
    }

    private void Start()
    {
        ChangeState(GameState.MainMenu);
        UIManager.Instance.OpenUI<CanvasMainMenu>();
    }

    #region CoreGame
    
    public void StartGame()
    {
        StartCoroutine(TransitionLv(0.7f));
    }

    private IEnumerator TransitionLv(float delay) // HIỆU ỨNG CHUYỂN CẢNH
    {
        transitionAnimator.SetTrigger(GameCONST.Anim_CLOUD_IN);
        yield return new WaitForSeconds(delay);

        UIManager.Instance.CloseAllUI();
        BoosterManager.Instance.ResetBoosters();
        LevelManager.Instance.PreLoadLevel();
        
        transitionAnimator.SetTrigger(GameCONST.Anim_CLOUD_OUT);
        
        yield return new WaitForSeconds(delay);
        
        LevelManager.Instance.OnLoadLevel();
        ChangeState(GameState.GamePlay);    
        UIManager.Instance.OpenUI<CanvasGamePlay>();

    }
    public void EndGame(bool isVictory)
    {
        if (IsState(GameState.GamePlay))
        {
            ChangeState(GameState.Finish);
            UIManager.Instance.CloseUI<CanvasGamePlay>(0f);
            ScoreManager scoreManager = LevelManager.Instance.ScoreManager;
            Timer timer = LevelManager.Instance.Timer;

            if (isVictory)
            {
                LevelScoreResult result = scoreManager.CalScoreOnWin(timer.RemainingTime);
                UIManager.Instance.OpenUI<CanvasVictory>().SetBaseInfo(result);
            }
            else
            {
                LevelManager.Instance.ResetPlayedLevel();
                LevelScoreResult result = scoreManager.CalScoreOnLose();
                UIManager.Instance.OpenUI<CanvasFail>().SetBaseScore(result);
            }
        }
    }
    #endregion
}