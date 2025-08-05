using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
        [Header("setup")] 
        [SerializeField, Range(0, 1f)]  protected float sliderValue;
        [SerializeField] private bool isMusicSlider = false;
        private bool previousValue;
        private bool currentValue;
        private Slider slider;

        [Header("Animation")] 
        [SerializeField, Range(0, 1f)] private float animationDuration = 0.5f;
        [SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Coroutine animateSliderCoroutine;

        [Header("Events")] 
        [SerializeField] private UnityEvent onToggleOn;
        [SerializeField] private UnityEvent onToggleOff;
        
        protected virtual void OnValidate()
        {
            SetupToggleComponents();

            slider.value = sliderValue;
        }

        private void SetupToggleComponents()
        {
            if (slider != null)
                return;

            OnInit();
        }

        private void OnInit()
        {
            slider = GetComponent<Slider>();

            if (slider == null)
            {
                Debug.Log("No slider found!", this);
                return;
            }

            // Lấy trạng thái từ SoundManager
            bool isOn = isMusicSlider
                ? SoundManager.Instance.GetMusicState()
                : SoundManager.Instance.GetFxState();

            currentValue = isOn;
            sliderValue = isOn ? 1f : 0f;

            // Setup slider ban đầu
            slider.value = sliderValue;
            slider.interactable = false;
            var sliderColors = slider.colors;
            sliderColors.disabledColor = Color.white;
            slider.colors = sliderColors;
            slider.transition = Selectable.Transition.None;

            // Gắn sự kiện bật/tắt
            if (isMusicSlider)
            {
                onToggleOn.AddListener(() => SoundManager.Instance.ToggleAllMusic(true));
                onToggleOff.AddListener(() => SoundManager.Instance.ToggleAllMusic(false));
            }
            else
            {
                onToggleOn.AddListener(() => SoundManager.Instance.ToggleAllFx(true));
                onToggleOff.AddListener(() => SoundManager.Instance.ToggleAllFx(false));
            }

            // Cập nhật UI bằng animation tương ứng
            SetStateAndStartAnimation(currentValue);
        }


        protected virtual void Awake()
        {
            OnInit();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Toggle();
        }

        
        private void Toggle()
        {
            SoundManager.Instance.PlayFx(FxID.Pop);
            SetStateAndStartAnimation(!currentValue);
        }
        
        private void SetStateAndStartAnimation(bool state)
        {
            previousValue = currentValue;
            currentValue = state;

            if (previousValue != currentValue)
            {
                if (currentValue)
                    onToggleOn?.Invoke();
                else
                    onToggleOff?.Invoke();
            }

            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (animateSliderCoroutine != null)
            {
                StopCoroutine(animateSliderCoroutine);
            }

            animateSliderCoroutine = StartCoroutine(AnimateSlider());
        }


        private IEnumerator AnimateSlider()
        {
            float startValue = slider.value;
            float endValue = currentValue ? 1 : 0;

            float time = 0;
            if (animationDuration > 0)
            {
                while (time < animationDuration)
                {
                    time += Time.deltaTime;

                    float lerpFactor = slideEase.Evaluate(time / animationDuration);
                    slider.value = sliderValue = Mathf.Lerp(startValue, endValue, lerpFactor);
                        
                    yield return null;
                }
            }
            slider.value = endValue;
        }
    }
