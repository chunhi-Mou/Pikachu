using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGamePlay : UICanvas
{
    [Header("Level Info")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Slider timeSlider;

    public override void Setup()
    {
        base.Setup();
        GameManager.Instance.OnScoreUpdate += UpdateScore;
        GameManager.Instance.OnTimerUpdate += UpdateTimer;
        
        timeSlider.maxValue = GameManager.Instance.LevelTime;
    }
    
    public override void Close(float delayTime)
    {
        base.Close(delayTime);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreUpdate -= UpdateScore;
            GameManager.Instance.OnTimerUpdate -= UpdateTimer;
        }
    }

    public void SettingsButton()
    {
        UIManager.Instance.OpenUI<CanvasSettings>().SetState(this);
    }

    private void UpdateScore(int newScore)
    {
        scoreText.text = newScore.ToString();
    }

    private void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
        timeSlider.value = time;
    }
}