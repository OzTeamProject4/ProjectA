using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPreviewPopupView : BaseUI
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Image _iconImage;

    [SerializeField] private StatItemView[] _statRows;

    [SerializeField] private Button _blockerButton;

    [SerializeField] private RectTransform _cardRect;

    private ItemPreviewPopupViewModel _viewModel;
    private bool _isSubscribed;

    private readonly List<string> _loadedSpriteKeys = new();

    public event Action OnCloseButtonClicked;

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
        ReleaseAllSprites();
    }

    public void Bind(ItemPreviewPopupViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("Bind: ItemPreviewPopupViewModel 이 null 입니다.");
            return;
        }

        Unsubscribe();
        ReleaseAllSprites();

        _viewModel = viewModel;

        LoadIconAsync().Forget();

        Subscribe();
    }

    public void MoveCardTo(Vector3 worldPosition)
    {
        if (null == _cardRect)
        {
            Debug.LogWarning("[ItemPreviewPopupView] _cardRect 가 연결되지 않았습니다.");
            return;
        }

        _cardRect.pivot = new Vector2(1f, 0.5f);
        _cardRect.position = worldPosition;
    }

    private void Subscribe()
    {
        if (_isSubscribed || null == _viewModel)
        {
            return;
        }

        if (null != _blockerButton)
        {
            _blockerButton.onClick.AddListener(HandleClickClose);
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

        if (null != _blockerButton)
        {
            _blockerButton.onClick.RemoveListener(HandleClickClose);
        }

        _isSubscribed = false;
    }

    private void RefreshDisplay()
    {
        if (null == _viewModel)
        {
            return;
        }

        _nameText.text = _viewModel.Name;

        if (null != _descriptionText)
        {
            _descriptionText.text = _viewModel.Description;
        }

        RefreshStatRows();
    }

    private void RefreshStatRows()
    {
        if (null == _statRows)
        {
            return;
        }

        IReadOnlyList<StatValue> values = _viewModel.GetStatValues();

        for (int i = 0; i < _statRows.Length; i++)
        {
            if (null == _statRows[i])
            {
                continue;
            }

            if (i >= values.Count)
            {
                _statRows[i].Hide();
                continue;
            }

            StatValue value = values[i];
            _statRows[i].SetValue(value.Type, value.Value, value.IsInteger);
        }
    }

    private async UniTaskVoid LoadIconAsync()
    {
        if (null == _iconImage)
        {
            return;
        }

        string spritePath = _viewModel.SpritePath;

        if (string.IsNullOrEmpty(spritePath))
        {
            _iconImage.enabled = false;
            return;
        }

        try
        {
            Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(spritePath, destroyCancellationToken);

            if (null == sprite)
            {
                _iconImage.enabled = false;
                return;
            }

            _loadedSpriteKeys.Add(spritePath);

            _iconImage.enabled = true;
            _iconImage.sprite = sprite;
        }
        catch (OperationCanceledException)
        {
            // 오브젝트 파괴로 취소됨, 무시
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[ItemPreviewPopupView] 스프라이트 로드 실패. spritePath={spritePath}\n{exception}");
        }
    }

    private void HandleClickClose()
    {
        OnCloseButtonClicked?.Invoke();
    }

    private void ReleaseAllSprites()
    {
        foreach (string key in _loadedSpriteKeys)
        {
            GameManager.Instance.ResourceManager.ReleaseAsset(key);
        }

        _loadedSpriteKeys.Clear();
    }
}