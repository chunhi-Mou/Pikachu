using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BaseButton : MonoBehaviour
{
    [SerializeField] protected Button button;
    [SerializeField] protected Image icon;
    [SerializeField] protected GameObject countIcon;
    [SerializeField] protected float buttonShiftY = -6f;

    private RectTransform iconRT;

    protected virtual void Awake()
    {
        iconRT = icon.rectTransform;
    }

    protected virtual void OnEnable()
    {
        ResetButtonVisual();
        AssignEvent();
    }

    protected virtual void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    protected virtual void AssignEvent()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    protected virtual void OnButtonClicked()
    {
        ShiftButtonIcon(buttonShiftY);
        if (button.interactable)
        {
            StartCoroutine(ShiftBackCoroutine(buttonShiftY * -1));
        }
    }

    protected virtual void SetButtonVisual(bool enabled)
    {
        button.interactable = enabled;
        if (countIcon != null)
        {
            countIcon.SetActive(enabled);
        }

        if (enabled)
        {
            icon.color = Color.white;
        }
        else
        {
            icon.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }

    private void ShiftButtonIcon(float shiftY)
    {
        if (iconRT != null)
        {
            iconRT.anchoredPosition = new Vector2(iconRT.anchoredPosition.x, iconRT.anchoredPosition.y + shiftY);
        }
    }

    private IEnumerator ShiftBackCoroutine(float shiftY)
    {
        yield return new WaitForSeconds(0.2f);
        ShiftButtonIcon(shiftY);
    }

    private void ResetButtonVisual()
    {
        if (iconRT != null)
        {
            iconRT.anchoredPosition = new Vector2(iconRT.anchoredPosition.x, 0); 
        }
        SetButtonVisual(true);
    }
}