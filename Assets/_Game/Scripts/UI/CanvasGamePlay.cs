using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGamePlay : UICanvas
{
    [Header("Level Info")]
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Animator scoreAnimator;
    [SerializeField] private Animator clockAnimator;
    
    private bool hasPlayedWarning = false;
    private float levelTime = 90f;
    public override void Setup()
    {
        base.Setup();
        InitializeUI();
    }

    private void InitializeUI()
    {
        levelTime = LevelManager.Instance.LevelTime;
        timeSlider.maxValue = levelTime;
        timeSlider.value = levelTime;
        hasPlayedWarning = false;
        UpdateScore(0);
        UpdateTimer(levelTime);
        AnimatorUtils.ChangeAnimUI(GameCONST.Anim_SCORE_UI_NONE, scoreAnimator);
        AnimatorUtils.ChangeAnimUI(GameCONST.Anim_CLOCK_NONE, clockAnimator);
        BoosterManager.Instance.ResetBoosters();
    }

    public void SettingsButton()
    {
        UIManager.Instance.OpenUI<CanvasSettings>().SetState(SettingsContext.FromGameplay);
    }

    public void UpdateScore(int newScore)
    {
        scoreText.text = newScore.ToString("N0");
        if (newScore != 0)
        {
            SoundManager.Instance.PlayFx(FxID.MatchSuccess);
            AnimatorUtils.ChangeAnimUI(GameCONST.Anim_SCORE_UI_UPDATE, scoreAnimator);
        }
    }
    
    public void UpdateTimer(float timeRemaining)
    {
        // Clamp để ko âm
        timeRemaining = Mathf.Max(0, timeRemaining);
        
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        
        timerText.text = $"{minutes:00}:{seconds:00}";
        timeSlider.value = timeRemaining;
        
        // Visual feedback khi gần hết thời gian
        if (timeRemaining <= 10f && timeRemaining > 0)
        {
            if (!hasPlayedWarning)
            {
                SoundManager.Instance.PlayFx(FxID.TimeUp);
                hasPlayedWarning = true;
                AnimatorUtils.ChangeAnimUI(GameCONST.Anim_CLOCK_TIMEUP, clockAnimator);
            }
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }
    }
}