using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//TODO 버튼이 늘어나면(Setting/Home) 여기에 필드와 이벤트를 추가
public class TopbarView : MonoBehaviour
{
    private const string CurrencyCountFormat = "N0";

    [SerializeField] private Button _backButton;

    [Header("Currency")]
    [SerializeField] private TMP_Text _goldCountText;
    [SerializeField] private TMP_Text _crystalCountText;

    public event Action OnBackClicked;

    private TopbarViewModel _topbarViewModel;

    private void Awake()
    {
        UnityUtil.ValidateReference(_backButton, nameof(TopbarView), nameof(_backButton));
        UnityUtil.ValidateReference(_goldCountText, nameof(TopbarView), nameof(_goldCountText));
        UnityUtil.ValidateReference(_crystalCountText, nameof(TopbarView), nameof(_crystalCountText));

        _topbarViewModel = new TopbarViewModel();
    }

    private void OnEnable()
    {
        _backButton.onClick.AddListener(HandleBackButtonClicked);

        _topbarViewModel.PropertyChanged += OnPropertyChanged;
        _topbarViewModel.Refresh();
    }

    private void OnDisable()
    {
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

    private void HandleBackButtonClicked()
    {
        if (OnBackClicked == null)
        {
            return;
        }

        OnBackClicked.Invoke();
    }
}
