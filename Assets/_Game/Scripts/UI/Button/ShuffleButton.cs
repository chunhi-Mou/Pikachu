using TMPro;
using UnityEngine;

public class ShuffleButton : BaseButton
{
    [SerializeField] private TextMeshProUGUI countShuffleUsed;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.OnShuffleCntUpdate += UpdateShuffleCount;
        UpdateShuffleCount(GameManager.Instance.ShuffleUse); 
    }

    protected override void OnButtonClicked()
    {
        base.OnButtonClicked();
        GameManager.Instance.UseShuffle();
    }

    private void UpdateShuffleCount(int count)
    {
        if (count <= 0)
        {
            SetButtonVisual(false);
        }
        else
        {
            SetButtonVisual(true);
            countShuffleUsed.text = count.ToString();
        }
    }
}