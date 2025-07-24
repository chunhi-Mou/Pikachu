using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasMainMenu : UICanvas
{
    [SerializeField] TextMeshProUGUI highestScoreText;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    
    private void Awake()
    {
        Setup();
    }

    public override void Setup()
    {
        currentLevelText.text = "Level: " + DataManager.Instance.GetHighestLevel();
        highestScoreText.text = DataManager.Instance.GetHighestScore().ToString();
    }
    public void PlayButton()
    {
        GameManager.Instance.StartGame(); 
    }

    public void SettingsButton()
    {
        UIManager.Instance.OpenUI<CanvasSettings>().SetState(this);
    }
}
