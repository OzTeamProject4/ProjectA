using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartySelectSlotView : MonoBehaviour
{
    [SerializeField] private Image _portraitImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private GameObject[] _starIcons;
    [SerializeField] private Button _selectButton;

    [Header("편성 표시")]
    [SerializeField] private GameObject _assignedMark;
    [SerializeField] private Color _assignedPortraitColor = new Color(0.45f, 0.45f, 0.45f, 1f);

    private PartySelectSlotViewModel _viewModel;
    private bool _isSubscribed;

    private Color _defaultPortraitColor = Color.white;
    private bool _hasCachedPortraitColor;

    public event Action<string> OnClicked;

    private void Awake()
    {
        if (null != _portraitImage)
        {
            _defaultPortraitColor = _portraitImage.color;
            _hasCachedPortraitColor = true;
        }
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void Bind(PartySelectSlotViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("[PartySelectItemView] Bind: viewModel 이 null 입니다.");
            return;
        }

        Unsubscribe();

        _viewModel = viewModel;

        Subscribe();
    }

    private void Subscribe()
    {
        if (_isSubscribed || null == _viewModel)
        {
            return;
        }

        if (null != _selectButton)
        {
            _selectButton.onClick.AddListener(HandleClickSelect);
        }

        _isSubscribed = true;

        RefreshDisplay();
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
        {
            return;
        }

        if (null != _selectButton)
        {
            _selectButton.onClick.RemoveListener(HandleClickSelect);
        }

        _isSubscribed = false;
    }

    private void RefreshDisplay()
    {
        if (null == _viewModel)
        {
            return;
        }

        if (null != _nameText)
        {
            _nameText.text = _viewModel.Name;
        }

        RefreshStar(_viewModel.Star);
        RefreshAssignedMark();
        LoadPortraitAsync(_viewModel.IconPath).Forget();
    }

    private void RefreshAssignedMark()
    {
        bool isAssigned = _viewModel.IsAssigned;

        if (null != _assignedMark)
        {
            _assignedMark.SetActive(isAssigned);
        }

        if (null != _portraitImage && _hasCachedPortraitColor)
        {
            _portraitImage.color = isAssigned ? _assignedPortraitColor : _defaultPortraitColor;
        }
    }

    private async UniTaskVoid LoadPortraitAsync(string iconPath)
    {
        if (null == _portraitImage)
        {
            return;
        }

        if (string.IsNullOrEmpty(iconPath))
        {
            _portraitImage.enabled = false;
            return;
        }

        Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(iconPath, destroyCancellationToken);

        if (null == sprite)
        {
            _portraitImage.enabled = false;
            return;
        }

        _portraitImage.sprite = sprite;
        _portraitImage.enabled = true;
    }

    private void RefreshStar(int targetCount)
    {
        if (null == _starIcons)
        {
            return;
        }

        for (int index = 0; index < _starIcons.Length; index++)
        {
            if (null == _starIcons[index])
            {
                continue;
            }

            _starIcons[index].SetActive(index < targetCount);
        }
    }

    private void HandleClickSelect()
    {
        if (null == _viewModel)
        {
            return;
        }

        OnClicked?.Invoke(_viewModel.DataId);
    }
}
