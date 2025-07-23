using System;
using UnityEngine;

public enum GameState { MainMenu, GamePlay, Finish, Revive, Setting, Pause }

public class GameManager : Singleton<GameManager>
{
    private static GameState gameState;

    public static void ChangeState(GameState state)
    {
        gameState = state;
    }

    public static bool IsState(GameState state) => gameState == state;

    [Header("GamePlay Settings")] 
    [SerializeField] private float levelTime = 90f;
    public float LevelTime => levelTime;
    private float timer;
    private int score;
    public event Action<float> OnTimerUpdate;
    public event Action<int> OnScoreUpdate;
    
    [Header("Booster Settings")]
    [SerializeField] private int maxShuffleUse = 1;
    [SerializeField] private int maxHintUse = 1;

    public int ShuffleUse { get; private set; }
    public int HintUse { get; private set; }

    public event Action<int> OnShuffleCntUpdate;
    public event Action<int> OnHintCntUpdate;

    public void ResetBoosters()
    {
        ShuffleUse = maxShuffleUse;
        HintUse = maxHintUse;
        OnShuffleCntUpdate?.Invoke(ShuffleUse);
        OnHintCntUpdate?.Invoke(HintUse);
    }

    public bool UseShuffle()
    {
        if (ShuffleUse <= 0) return false;
        ShuffleUse--;
        OnShuffleCntUpdate?.Invoke(ShuffleUse);
        return true;
    }

    public bool UseHint()
    {
        if (HintUse <= 0) return false;
        HintUse--;
        OnHintCntUpdate?.Invoke(HintUse);
        return true;
    }
    public void ResetTotalScore()
    {
        PlayerPrefs.DeleteKey(GameCONST.SCORE);
    }
    public int GetTotalScore()
    {
        return PlayerPrefs.GetInt(GameCONST.SCORE, 0);
    }
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
        //TODO: Setup lai
        //ChangeState(GameState.MainMenu);
        //IManager.Instance.OpenUI<CanvasMainMenu>();
        StartGame();
    }

    private void Update()
    {
        if (IsState(GameState.GamePlay))
        {
            timer -= Time.deltaTime;
            OnTimerUpdate?.Invoke(timer); // Phát sự kiện cho UI

            if (timer <= 0)
            {
                timer = 0;
                EndGame(false); // Hết giờ -> Thua
            }
        }
    }
    public void StartGame()
    {
        ChangeState(GameState.GamePlay);
        score = 0;
        timer = levelTime;
        // Mở màn hình chơi game
        UIManager.Instance.CloseAllUI();
        UIManager.Instance.OpenUI<CanvasGamePlay>();
        
        // Cập nhật giao diện lúc bắt đầu
        OnScoreUpdate?.Invoke(score);
        OnTimerUpdate?.Invoke(timer);
        ResetBoosters();
    }
    public void AddScore(int amount)
    {
        if (!IsState(GameState.GamePlay)) return;
        score += amount;
        OnScoreUpdate?.Invoke(score);
    }

    public void EndGame(bool isVictory)
    {
        ChangeState(GameState.Finish);
        UIManager.Instance.CloseUI<CanvasGamePlay>(0f);
        SaveTotalScore(score);
        if (isVictory)
        {
            UIManager.Instance.OpenUI<CanvasVictory>().SetBaseInfo(GetTotalScore(), ((int)timer).ToString());
        }
        else
        {
            UIManager.Instance.OpenUI<CanvasFail>().SetBaseScore(score);
        }
    }
    private void SaveTotalScore(int currentScore)
    {
        int totalScore = PlayerPrefs.GetInt(GameCONST.SCORE, 0);
        totalScore += currentScore;
        PlayerPrefs.SetInt(GameCONST.SCORE, totalScore);
        PlayerPrefs.Save();
    }
}