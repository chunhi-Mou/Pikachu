using System;
using UnityEngine;

public enum GameState { MainMenu, GamePlay, Finish, Revive, Setting, Pause }

public class GameManager : Singleton<GameManager> //TODO: Refactor
{
    private static GameState gameState;

    public static void ChangeState(GameState state)
    {
        gameState = state;
    }

    public static bool IsState(GameState state) => gameState == state;
    
    private void Awake()
    {
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
        ChangeState(GameState.GamePlay);
        
        //Đóng tất cả
        UIManager.Instance.CloseAllUI();
        
        BoosterManager.Instance.ResetBoosters();
        // GameManager gọi LevelManager để tải màn chơi
        LevelManager.Instance.PreLoadLevel();
        LevelManager.Instance.OnLoadLevel(); 
        
        // Mở màn hình chơi game
        UIManager.Instance.OpenUI<CanvasGamePlay>();
    }
    public void EndGame(bool isVictory)
    {
        if (IsState(GameState.GamePlay))
        {
            ChangeState(GameState.Finish);
            UIManager.Instance.CloseUI<CanvasGamePlay>(0f);
            Score score = LevelManager.Instance.Score;
            Timer timer = LevelManager.Instance.Timer;

            if (isVictory)
            {
                LevelScoreResult result = score.CalScoreOnWin(timer.RemainingTime);
                UIManager.Instance.OpenUI<CanvasVictory>().SetBaseInfo(result);
            }
            else
            {
                LevelManager.Instance.ResetPlayedLevel();
                LevelScoreResult result = score.CalScoreOnLose();
                UIManager.Instance.OpenUI<CanvasFail>().SetBaseScore(result);
            }
        }
    }
    #endregion
}