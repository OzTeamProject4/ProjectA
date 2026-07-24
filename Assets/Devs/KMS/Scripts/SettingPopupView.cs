using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingPopupView : BaseUI, IPointerClickHandler
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Image _dimmerImage;
    private const float DefaultVolume = 0.5f;

    private void OnEnable()
    {
        ResetRectTransform();

        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(ClosePopup);
        }

        if (_resetButton != null)
        {
            _resetButton.onClick.AddListener(ResetVolume);
        }

        if (_bgmSlider != null)
        {
            _bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        }

        if (_sfxSlider != null)
        {
            _sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }
    }

    private void OnDisable()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveListener(ClosePopup);
        }

        if (_resetButton != null)
        {
            _resetButton.onClick.RemoveListener(ResetVolume);
        }

        if (_bgmSlider != null)
        {
            _bgmSlider.onValueChanged.RemoveListener(OnBgmVolumeChanged);
        }

        if (_sfxSlider != null)
        {
            _sfxSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
        }
    }

    private void ClosePopup()
    {
        GameManager.Instance.UIManager.CloseSettingPopup();
    }

    private void OnBgmVolumeChanged(float volume)
    {
        GameManager.Instance.AudioManager.SetBgmVolume(volume);
    }

    private void OnSfxVolumeChanged(float volume)
    {
        GameManager.Instance.AudioManager.SetSfxVolume(volume);
    }

    private void ResetVolume()
    {
        if (_bgmSlider != null)
        {
            _bgmSlider.value = DefaultVolume;
        }

        if (_sfxSlider != null)
        {
            _sfxSlider.value = DefaultVolume;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_dimmerImage == null)
        {
            return;
        }

        if (eventData.pointerCurrentRaycast.gameObject != _dimmerImage.gameObject)
        {
            return;
        }

        ClosePopup();
    }

    private void ResetRectTransform()
    {
        if (transform is not RectTransform rectTransform)
        {
            return;
        }

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}