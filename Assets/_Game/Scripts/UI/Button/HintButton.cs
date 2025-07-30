using TMPro;
using UnityEngine;

public class HintButton : BaseButton
{
    [Header("Hint Specific")]
    [SerializeField] private TextMeshProUGUI boosterCntText;

    protected override void RegisterEvents()
    {
        BoosterManager.OnHintCountChanged += UpdateHintDisplay;
        UpdateHintDisplay(BoosterManager.Instance.CurrentHintUse);
    }

    protected override void UnregisterEvents()
    {
        BoosterManager.OnHintCountChanged -= UpdateHintDisplay;
    }

    protected override bool ExecuteButtonAction()
    {
        return BoosterManager.Instance.TryUseHint();
    }

    private void UpdateHintDisplay(int count)
    {
        bool hasHints = count > 0;
        SetButtonState(hasHints, count.ToString());
    }

    protected override void UpdateCountDisplay(string countText)
    {
        boosterCntText.text = countText;
    }
}