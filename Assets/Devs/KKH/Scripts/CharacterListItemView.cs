using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterListItemView : MonoBehaviour
{
    [SerializeField] private Image _portraitImage;
    [SerializeField] private TMP_Text _nameText;

    [SerializeField] private Image _starIconPrefab;
    [SerializeField] private Transform _starIconContainer;

    [SerializeField] private Button _selectButton;

    private readonly List<GameObject> _spawnedStarIcons = new();

    public event Action<string> OnClicked;

    private CharacterListItemViewModel _viewModel;
    private bool _isSubscribed;

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    public void Bind(CharacterListItemViewModel viewModel, CharacterDisplayInfo displayInfo)
    {
        if (null == viewModel)
        {
            Debug.LogError("Bind: CharacterListItemViewModel 이 null 입니다.");
            return;
        }

        Unsubscribe();

        _viewModel = viewModel;
        _nameText.text = displayInfo.Name;
        _portraitImage.sprite = displayInfo.Portrait;

        TrySubscribe();
    }

    private void TrySubscribe()
    {
        if (_isSubscribed || null == _viewModel || !isActiveAndEnabled)
        {
            return;
        }

        _viewModel.OnStarChanged += HandleStarChanged;

        if (null != _selectButton)
        {
            _selectButton.onClick.AddListener(HandleClickSelect);
        }

        _viewModel.Initialize();
        RefreshStar();

        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed || null == _viewModel)
        {
            return;
        }

        _viewModel.OnStarChanged -= HandleStarChanged;
        _viewModel.Dispose();

        if (null != _selectButton)
        {
            _selectButton.onClick.RemoveListener(HandleClickSelect);
        }

        _isSubscribed = false;
    }

    private void HandleStarChanged()
    {
        RefreshStar();
    }

    private void RefreshStar()
    {
        if (null == _starIconPrefab || null == _starIconContainer)
        {
            Debug.LogWarning("RefreshStar: 별 아이콘 프리팹 또는 컨테이너가 연결되지 않았습니다.");
            return;
        }

        ClearStarIcons();

        for (int i = 0; i < _viewModel.CurrentStar; i++)
        {
            Image icon = Instantiate(_starIconPrefab, _starIconContainer);
            _spawnedStarIcons.Add(icon.gameObject);
        }
    }

    private void ClearStarIcons()
    {
        foreach (GameObject icon in _spawnedStarIcons)
        {
            if (null != icon)
            {
                Destroy(icon);
            }
        }

        _spawnedStarIcons.Clear();
    }

    private void HandleClickSelect()
    {
        OnClicked?.Invoke(_viewModel.CharacterId);
    }
}