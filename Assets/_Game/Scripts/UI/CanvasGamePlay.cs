using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGamePlay : UICanvas
{
    [Header("Level Info")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Slider timeSlider;
    
    [Header("Booster")]
    [SerializeField] TextMeshProUGUI countShuffleUsed;
    [SerializeField] TextMeshProUGUI countHintUsed;
    [SerializeField] Button hintButton;
    [SerializeField] Button shuffleButton;
    [SerializeField] GameObject shuffleCntIcon;
    [SerializeField] GameObject hintCntIcon;
    [SerializeField] Image shuffleIcon;
    [SerializeField] Image hintIcon;

    private RectTransform shuffleIconRT;
    private RectTransform hintIconRT;
    
    public override void Setup()
    {
        base.Setup();
        GameManager.Instance.OnScoreUpdate += UpdateScore;
        GameManager.Instance.OnTimerUpdate += UpdateTimer;
        GameManager.Instance.OnHintUsed += UpdateCountHintUsed;
        GameManager.Instance.OnShuffleUsed += UpdateShuffleTimerUsed;
        
        shuffleIconRT = shuffleIcon.rectTransform;
        hintIconRT = hintIcon.rectTransform;

        shuffleButton.onClick.AddListener(() => ShiftButtonIcon(shuffleIconRT));
        hintButton.onClick.AddListener(() => ShiftButtonIcon(hintIconRT));
        
        timeSlider.maxValue = GameManager.Instance.LevelTime;
    }
    
    public override void Close(float delayTime)
    {
        base.Close(delayTime);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreUpdate -= UpdateScore;
            GameManager.Instance.OnTimerUpdate -= UpdateTimer;
            GameManager.Instance.OnHintUsed -= UpdateCountHintUsed;
            GameManager.Instance.OnShuffleUsed -= UpdateShuffleTimerUsed;
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
    private void UpdateShuffleTimerUsed(int count)
    {
        if (count <= 0)
        {
            shuffleButton.interactable = false;
            shuffleCntIcon.SetActive(false);
            shuffleIcon.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            return;
        }
        countShuffleUsed.text = count.ToString();
    }

    private void UpdateCountHintUsed(int count)
    {
        if (count <= 0)
        {
            hintButton.interactable = false;
            hintCntIcon.SetActive(false);
            hintIcon.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            return;
        }
        countHintUsed.text = count.ToString();
    }
    private void ShiftButtonIcon(RectTransform rt, float shiftY = -6f)
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + shiftY);
    }
}
