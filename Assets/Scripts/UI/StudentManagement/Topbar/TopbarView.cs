using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopbarView : MonoBehaviour
{
    private const string CurrencyCountFormat = "N0";

    [SerializeField] private Button _backButton;

    [Header("Currency")]
    [SerializeField] private Image _goldIconImage;
    [SerializeField] private TMP_Text _goldCountText;
    [SerializeField] private Image _crystalIconImage;
    [SerializeField] private TMP_Text _crystalCountText;

    public event Action OnBackClicked;

    private TopbarViewModel _topbarViewModel;

    private CancellationTokenSource _disableCts;

    private void Awake()
    {
        UnityUtil.ValidateReference(_backButton, nameof(TopbarView), nameof(_backButton));
        UnityUtil.ValidateReference(_goldIconImage, nameof(TopbarView), nameof(_goldIconImage));
        UnityUtil.ValidateReference(_goldCountText, nameof(TopbarView), nameof(_goldCountText));
        UnityUtil.ValidateReference(_crystalIconImage, nameof(TopbarView), nameof(_crystalIconImage));
        UnityUtil.ValidateReference(_crystalCountText, nameof(TopbarView), nameof(_crystalCountText));

        _topbarViewModel = new TopbarViewModel();
    }

    private void OnEnable()
    {
        _disableCts = new CancellationTokenSource();

        _backButton.onClick.AddListener(HandleBackButtonClicked);

        _topbarViewModel.PropertyChanged += OnPropertyChanged;
        _topbarViewModel.Refresh();
    }

    private void OnDisable()
    {
        if (_disableCts != null)
        {
            _disableCts.Cancel();
            _disableCts.Dispose();
            _disableCts = null;
        }

        _backButton.onClick.RemoveListener(HandleBackButtonClicked);

        _topbarViewModel.PropertyChanged -= OnPropertyChanged;
    }

    private void OnDestroy()
    {
        _topbarViewModel.Dispose();
        _topbarViewModel = null;
    }

    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_topbarViewModel.GoldCount):
                UpdateGoldCountText();
                break;
            case nameof(_topbarViewModel.CrystalCount):
                UpdateCrystalCountText();
                break;
            case nameof(_topbarViewModel.GoldIconKey):
                UpdateGoldIconAsync().Forget();
                break;
            case nameof(_topbarViewModel.CrystalIconKey):
                UpdateCrystalIconAsync().Forget();
                break;
        }
    }

    private void UpdateGoldCountText()
    {
        _goldCountText.text = _topbarViewModel.GoldCount.ToString(CurrencyCountFormat);
    }

    private void UpdateCrystalCountText()
    {
        _crystalCountText.text = _topbarViewModel.CrystalCount.ToString(CurrencyCountFormat);
    }

    private UniTask UpdateGoldIconAsync()
    {
        return SpriteLoader.LoadIntoAsync(_goldIconImage, _topbarViewModel.GoldIconKey, _disableCts.Token);
    }

    private UniTask UpdateCrystalIconAsync()
    {
        return SpriteLoader.LoadIntoAsync(_crystalIconImage, _topbarViewModel.CrystalIconKey, _disableCts.Token);
    }

    private void HandleBackButtonClicked()
    {
        if (OnBackClicked == null)
        {
            return;
        }

        OnBackClicked.Invoke();
    }
}
