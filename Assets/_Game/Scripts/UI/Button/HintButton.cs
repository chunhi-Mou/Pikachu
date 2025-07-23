using TMPro;
using UnityEngine;

public class HintButton : BaseButton
{
    [SerializeField] private TextMeshProUGUI countHintUsed;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.OnHintCntUpdate += UpdateHintCount;
        UpdateHintCount(GameManager.Instance.HintUse); 
    }

    protected override void OnButtonClicked()
    {
        base.OnButtonClicked();
        GameManager.Instance.UseHint();
    }

    private void UpdateHintCount(int count)
    {
        if (count <= 0)
        {
            SetButtonVisual(false);
        }
        else
        {
            SetButtonVisual(true);
            if (countHintUsed != null)
            {
                countHintUsed.text = count.ToString();
            }
        }
    }
}