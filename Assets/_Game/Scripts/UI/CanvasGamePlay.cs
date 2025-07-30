using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGamePlay : UICanvas
{
    [Header("Level Info")]
    [SerializeField] private Slider timeSlider;
    [SerializeField] private float levelTime = 90f;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;

    public override void Setup()
    {
        base.Setup();
        InitializeUI();
    }

    private void InitializeUI()
    {
        timeSlider.maxValue = levelTime;
        timeSlider.value = levelTime;
        UpdateScore(0);
        UpdateTimer(levelTime);
        
        // Reset boosters when starting new game
        BoosterManager.Instance.ResetBoosters();
    }

    public void SettingsButton()
    {
        UIManager.Instance.OpenUI<CanvasSettings>().SetState(this);
    }

    public void UpdateScore(int newScore)
    {
        scoreText.text = newScore.ToString("N0"); // Format với comma separator
    }

    public void UpdateTimer(float timeRemaining)
    {
        // Clamp để đảm bảo không âm
        timeRemaining = Mathf.Max(0, timeRemaining);
        
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        
        timerText.text = $"{minutes:00}:{seconds:00}";
        timeSlider.value = timeRemaining;
        
        // Visual feedback khi gần hết thời gian
        if (timeRemaining <= 10f && timeRemaining > 0)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }
    }
}