using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StudentGachaView : BaseUI
{
    [SerializeField] private StudentGachaListSlotView _slotPrefab;
    [SerializeField] private Transform _content;

    [Header("DrawButton")]
    [SerializeField] private SingleGachaDrawButton _singleGachaDrawButton;
    [SerializeField] private MultiGachaDrawButton _multiGachaDrawButton;

    [Header("ChangePart")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Image _portraitIconImage;
    [SerializeField] private Image _gachaMainImage;

    private string _startGachaId;

    private StudentGachaViewModel _gachaViewModel;

    private CancellationTokenSource _loadMainCts;
    private CancellationTokenSource _loadPortraitCts;

    private void Awake()
    {
        UnityUtil.ValidateReference(_singleGachaDrawButton, nameof(StudentGachaView), nameof(_singleGachaDrawButton));
        UnityUtil.ValidateReference(_multiGachaDrawButton, nameof(StudentGachaView), nameof(_multiGachaDrawButton));

        RefreshGachaList();

        _gachaViewModel = new StudentGachaViewModel();
    }

    private void OnEnable()
    {
        _gachaViewModel.OnEnable();
        _gachaViewModel.ModelPropertyChanged += OnModelPropertyChanged;

        _gachaViewModel.RequestUpdateGacha(_startGachaId);

        SubscribeEvents();
    }

    private void OnDisable()
    {
        if (_loadMainCts != null)
        {
            _loadMainCts.Cancel();
            _loadMainCts.Dispose();
            _loadMainCts = null;
        }

        if (_loadPortraitCts != null)
        {
            _loadPortraitCts.Cancel();
            _loadPortraitCts.Dispose();
            _loadPortraitCts = null;
        }

        _gachaViewModel.OnDisable();
        _gachaViewModel.ModelPropertyChanged -= OnModelPropertyChanged;

        UnsubscribeEvents();
    }

    private void OnDestroy()
    {
        _gachaViewModel.Dispose();
        _gachaViewModel = null;
    }

    private void RefreshGachaList()
    {
        if(!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, StudentGachaListData> studentGachaList))
        {
            Debug.LogError($"[{nameof(StudentGachaView)}:{nameof(RefreshGachaList)}] GachaListData 테이블을 불러오는데 실패했습니다.");
            return;
        }

        foreach (StudentGachaListData gachaListData in studentGachaList.Values)
        {
            StudentGachaListSlotView gachaListSlotView = Instantiate(_slotPrefab, _content);

            if (string.IsNullOrWhiteSpace(_startGachaId))
            {
                _startGachaId = gachaListData.GachaId;
            }

            gachaListSlotView.UpdateGachaBannerSlotAsync(gachaListData.GachaId, gachaListData.BackgroundKey, gachaListData.PortraitKey).Forget();
            gachaListSlotView.ButtonClicked += HandleSlotClicked;
        }
    }

    private void OnModelPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case (nameof(_gachaViewModel.Name)):
                UpdateNameText();
                break;
            case (nameof(_gachaViewModel.MainKey)):
                UpdateMainImageAsync().Forget();
                break;
            case (nameof(_gachaViewModel.PortraitKey)):
                UpdatePortraitImageAsync().Forget();
                break;
        }
    }

    public async UniTask UpdateMainImageAsync()
    {
        if (_loadMainCts != null)
        {
            _loadMainCts.Cancel();
            _loadMainCts.Dispose();
        }

        _loadMainCts = new CancellationTokenSource();

        Sprite mainSprite = await LoadSpriteAsync(_gachaViewModel.MainKey, _loadMainCts.Token);

        _gachaMainImage.sprite = mainSprite;
    }

    public void UpdateNameText()
    {
        _nameText.text = _gachaViewModel.Name;
    }

    public async UniTask UpdatePortraitImageAsync()
    {
        if (_loadPortraitCts != null)
        {
            _loadPortraitCts.Cancel();
            _loadPortraitCts.Dispose();
        }

        _loadPortraitCts = new CancellationTokenSource();

        Sprite portraitSprite = await LoadSpriteAsync(_gachaViewModel.PortraitKey, _loadPortraitCts.Token);

        _portraitIconImage.sprite = portraitSprite;
    }

    private void SubscribeEvents()
    {
        _singleGachaDrawButton.ButtonClicked += HandleSingleGachaDrawClicked;
        _multiGachaDrawButton.ButtonClicked += HandleMultiGachaDrawClicked;
    }

    private void UnsubscribeEvents()
    {
        _singleGachaDrawButton.ButtonClicked -= HandleSingleGachaDrawClicked;
        _multiGachaDrawButton.ButtonClicked -= HandleMultiGachaDrawClicked;
    }

    private void HandleSlotClicked(string gachaid)
    {
        _gachaViewModel.RequestUpdateGacha(gachaid);
    }

    private void HandleSingleGachaDrawClicked()
    {
        //1회 가챠 로직
    }

    private void HandleMultiGachaDrawClicked()
    {
        //10회 가챠 로직
    }

    private async UniTask<Sprite> LoadSpriteAsync(string key, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError($"[{nameof(StudentGachaView)}:{nameof(LoadSpriteAsync)}] 전달된 Sprite key가 null이거나 빈 문자열입니다.");
            return null;
        }

        Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(key, cancellationToken);

        if (sprite == null)
        {
            Debug.LogError($"[{nameof(StudentGachaView)}:{nameof(LoadSpriteAsync)}] '{key}' Sprite를 찾을 수 없습니다.");
            return null;
        }

        return sprite;
    }
}
