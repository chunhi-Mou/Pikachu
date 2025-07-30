using TMPro;
using UnityEngine;

public class ShuffleButton : BaseButton
{
    [Header("Shuffle Specific")]
    [SerializeField] private TextMeshProUGUI boosterCntText;

    protected override void RegisterEvents()
    {
        BoosterManager.OnShuffleCountChanged += UpdateShuffleDisplay;
        UpdateShuffleDisplay(BoosterManager.Instance.CurrentShuffleUse);
    }

    protected override void UnregisterEvents()
    {
        BoosterManager.OnShuffleCountChanged -= UpdateShuffleDisplay;
    }

    protected override bool ExecuteButtonAction()
    {
        return BoosterManager.Instance.TryUseShuffle();
    }

    private void UpdateShuffleDisplay(int count)
    {
        bool hasShuffles = count > 0;
        SetButtonState(hasShuffles, count.ToString());
    }

    protected override void UpdateCountDisplay(string countText)
    {
        boosterCntText.text = countText;
    }
}