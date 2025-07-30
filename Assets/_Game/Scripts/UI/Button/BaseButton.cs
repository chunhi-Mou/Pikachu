using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] protected Button button;
    [SerializeField] protected Image icon;
    [SerializeField] protected GameObject countIcon;
    
    [Header("Animation Settings")]
    [SerializeField] protected float buttonShiftY = -6f;
    [SerializeField] protected float animationDuration = 0.2f;

    private RectTransform iconRectTransform;
    private Vector2 originalIconPosition;
    private bool isAnimating = false;

    protected void Awake()
    {
        iconRectTransform = icon.rectTransform;
        originalIconPosition = iconRectTransform.anchoredPosition;
    }

    protected void OnEnable()
    {
        RegisterEvents();
        button.onClick.AddListener(OnButtonClicked);
        ResetIconPosition();
    }

    protected void OnDisable()
    {
        UnregisterEvents();
        button.onClick.RemoveListener(OnButtonClicked);
    }

    protected abstract void RegisterEvents();
    protected abstract void UnregisterEvents();

    private void OnButtonClicked()
    {
        if (!button.interactable || isAnimating) return;
        StartCoroutine(ButtonClickSequence());
    }

    private IEnumerator ButtonClickSequence()
    {
        isAnimating = true;
        // Icon xuong
        ShiftIcon(buttonShiftY);
        yield return new WaitForSeconds(animationDuration);
        bool actionSuccess = ExecuteButtonAction();
        bool shouldAnimateBack = actionSuccess && button.interactable;
        if (shouldAnimateBack)
        {
            ShiftIcon(-buttonShiftY);
        }
        isAnimating = false;
    }
    
    protected abstract bool ExecuteButtonAction();

    protected void SetButtonState(bool isEnabled, string countText = "")
    {
        button.interactable = isEnabled;
        if (countIcon != null)
        {
            countIcon.SetActive(isEnabled);
        }
        icon.color = isEnabled ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
        UpdateCountDisplay(countText);
        if (!isAnimating && isEnabled)
        {
            ResetIconPosition();
        }
    }

    protected virtual void UpdateCountDisplay(string countText)
    {
        // Override cho class con
    }

    private void ShiftIcon(float shiftY)
    {
        if (iconRectTransform != null)
        {
            var currentPos = iconRectTransform.anchoredPosition;
            iconRectTransform.anchoredPosition = new Vector2(currentPos.x, currentPos.y + shiftY);
        }
    }

    private void ResetIconPosition()
    {
        if (iconRectTransform != null)
        {
            iconRectTransform.anchoredPosition = originalIconPosition;
        }
    }
}
