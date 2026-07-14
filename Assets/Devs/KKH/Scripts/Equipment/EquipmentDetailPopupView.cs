using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDetailPopupView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Image _iconImage;

    [SerializeField] private StatItemView[] _statRows;

    [SerializeField] private Button _equipButton;
    [SerializeField] private Button _unequipButton;
    [SerializeField] private Button _closeButton;

    private EquipmentDetailPopupViewModel _viewModel;
    private bool _isSubscribed;

    private readonly List<string> _loadedSpriteKeys = new();

    public event Action OnEquipped;
    public event Action OnUnequipped;
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

    public void Bind(EquipmentDetailPopupViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("Bind: EquipmentDetailPopupViewModel 이 null 입니다.");
            return;
        }

        Unsubscribe();

        _viewModel = viewModel;

        LoadIconAsync().Forget();

        Subscribe();
    }

    private void Subscribe()
    {
        if (_isSubscribed || null == _viewModel)
        {
            return;
        }

        _equipButton.onClick.AddListener(HandleClickEquip);
        _unequipButton.onClick.AddListener(HandleClickUnequip);

        if (null != _closeButton)
        {
            _closeButton.onClick.AddListener(HandleClickClose);
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

        _equipButton.onClick.RemoveListener(HandleClickEquip);
        _unequipButton.onClick.RemoveListener(HandleClickUnequip);

        if (null != _closeButton)
        {
            _closeButton.onClick.RemoveListener(HandleClickClose);
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

        bool isEquipped = _viewModel.IsEquippedByThisCharacter;

        _equipButton.gameObject.SetActive(!isEquipped);
        _unequipButton.gameObject.SetActive(isEquipped);

        _equipButton.interactable = _viewModel.CanEquip;

        RefreshStatRows();
    }

    private void RefreshStatRows()
    {
        if (null == _statRows)
        {
            return;
        }

        IReadOnlyList<StatDelta> deltas = _viewModel.GetStatDelta();

        for (int i = 0; i < _statRows.Length; i++)
        {
            if (null == _statRows[i])
            {
                continue;
            }

            if (i >= deltas.Count)
            {
                _statRows[i].Hide();
                continue;
            }

            _statRows[i].SetDelta(deltas[i]);
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
            Debug.LogWarning($"[EquipmentDetailPopupView] 스프라이트 로드 실패. spritePath={spritePath}\n{exception}");
        }
    }

    private void HandleClickEquip()
    {
        _viewModel.EquipCommand();

        OnEquipped?.Invoke();
    }

    private void HandleClickUnequip()
    {
        _viewModel.UnequipCommand();

        OnUnequipped?.Invoke();
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