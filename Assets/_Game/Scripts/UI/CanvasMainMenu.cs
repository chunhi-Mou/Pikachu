using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasMainMenu : UICanvas
{
    [SerializeField] TextMeshProUGUI highestScoreText;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    
    private int currentLevel;

    void Start()
    {
        Setup();
    }

    public override void Setup()
    {
        currentLevel = PlayerPrefs.GetInt(GameCONST.HIGHEST_PLAYED_LEVEL, 1);
        currentLevelText.text = "Level: " + currentLevel.ToString();
        highestScoreText.text = PlayerPrefs.GetInt(GameCONST.HIGHEST_SCORE, 0).ToString();
    }
    public void PlayButton()
    {
        GameManager.Instance.StartGame(currentLevel); 
    }

    public void SettingsButton()
    {
        UIManager.Instance.OpenUI<CanvasSettings>().SetState(this);
    }
}
