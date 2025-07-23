using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField] bool isDestroyOnClose = false;

    private void Awake()
    {
        // Handle RABIT EARS
        RectTransform rectTransform = GetComponent<RectTransform>();
        float ratio = (float) Screen.width / (float) Screen.height;
        if (ratio > 2.1f) // Nếu tỉ lệ màn hình quá CAO
        {
            Vector2 leftBottomCorner = rectTransform.offsetMin; // traái dưới UI hiện tại
            Vector2 rightTopCorner = rectTransform.offsetMax; // trên phải UI hiện tại

            leftBottomCorner.y = 0f; // căn lại cho chuẩn
            rightTopCorner.y = -100f; // phần trên hạ xuống 100 pixel
            
            rectTransform.offsetMin = leftBottomCorner; // gán
            rectTransform.offsetMax = rightTopCorner;
        }
    }
    
    // Call this before canvas be active
    public virtual void Setup()
    {
        
    }
    // Call this RIGHT AFTER be active
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }
    // Close Canvas AFTER time(s)
    public virtual void Close(float time)
    {
        Invoke(nameof(CloseDirectly), time);
    }
    // Closed directly
    public virtual void CloseDirectly()
    {
        if (!isDestroyOnClose)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}