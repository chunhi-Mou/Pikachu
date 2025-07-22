using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGamePlay : UICanvas
{
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

    private void UpdateScore(int newScore)
    {
        scoreText.text = "Score: " + newScore.ToString();
    }

    private void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timeSlider.value = time;
    }

    public void SettingsButton()
    {
        //TODO: Tam dung Game
        UIManager.Instance.OpenUI<CanvasSettings>().SetState(this);
    }
}