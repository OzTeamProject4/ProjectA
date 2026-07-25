using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExperienceItemSlotView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private const float HoldDelaySeconds = 1.5f;
    private const float RepeatIntervalSeconds = 0.15f;

    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private Image _itemIconImage;
    [SerializeField] private TMP_Text _itemCountText;
    [SerializeField] private Button _slotButton;

    private ExperienceItemSlotViewModel _experienceItemSlotViewModel;
    
    private CancellationTokenSource _disableCts;
    private CancellationTokenSource _holdCts;

    public event Action<MaterialModel> OnSlotClicked;

    public void Awake()
    {
        UnityUtil.ValidateReference(_itemNameText, nameof(ExperienceItemSlotView), nameof(_itemNameText));
        UnityUtil.ValidateReference(_itemIconImage, nameof(ExperienceItemSlotView), nameof(_itemIconImage));
        UnityUtil.ValidateReference(_itemCountText, nameof(ExperienceItemSlotView), nameof(_itemCountText));
        UnityUtil.ValidateReference(_slotButton, nameof(ExperienceItemSlotView), nameof(_slotButton));

        _experienceItemSlotViewModel = new ExperienceItemSlotViewModel();
    }

    private void OnEnable()
    {
        _disableCts = new CancellationTokenSource();

        _experienceItemSlotViewModel.PropertyChanged += OnPropertyChanged;
    }

    private void OnDisable()
    {
        if (_disableCts != null)
        {
            _disableCts.Cancel();
            _disableCts.Dispose();
            _disableCts = null;
        }

        _experienceItemSlotViewModel.PropertyChanged -= OnPropertyChanged;
    }

    public void OnDestroy()
    {
        _experienceItemSlotViewModel.Dispose();
        _experienceItemSlotViewModel = null;
    }

    public void SetModel(MaterialModel materialModel)
    {
        _experienceItemSlotViewModel.SetModel(materialModel);
        _experienceItemSlotViewModel.Refresh();
    }

    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_experienceItemSlotViewModel.Name):
                UpdateNameText();
                break;
            case nameof(_experienceItemSlotViewModel.Count):
                UpdateCountText();
                break;
            case nameof(_experienceItemSlotViewModel.IconKey):
                UpdateIconImageAsync().Forget();
                break;
        }
    }

    //TODO 수정
    private void UpdateNameText()
    {
        _itemCountText.text = _experienceItemSlotViewModel.Count.ToString();
    }

    //TODO 수정
    private void UpdateCountText()
    {
        _itemCountText.text = _experienceItemSlotViewModel.Count.ToString();
    }

    private async UniTask UpdateIconImageAsync()
    {
        string iconKey = _experienceItemSlotViewModel.IconKey;

        if (string.IsNullOrWhiteSpace(iconKey))
        {
            return;
        }

        Sprite iconSprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(iconKey, _disableCts.Token);

        if (iconSprite == null)
        {
            return;
        }

        _itemIconImage.sprite = iconSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (null == _experienceItemSlotViewModel || _experienceItemSlotViewModel.Count <= 0)
        {
            return;
        }

        TriggerUse();
        StartHoldAsync().Forget();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CancelHold();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CancelHold();
    }

    private void TriggerUse()
    {
        if (OnSlotClicked == null)
        {
            return;
        }

        OnSlotClicked.Invoke(_experienceItemSlotViewModel.MaterialModel);
    }

    private async UniTaskVoid StartHoldAsync()
    {
        CancelHold();

        _holdCts = new CancellationTokenSource();
        CancellationToken token = _holdCts.Token;

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(HoldDelaySeconds), cancellationToken: token);

            while (!token.IsCancellationRequested)
            {
                //TODO 조건문 고민 (0이면 슬롯이 사라지지 않나?)
                if (null == _experienceItemSlotViewModel && _experienceItemSlotViewModel.Count <= 0)
                {
                    break;
                }

                TriggerUse();

                await UniTask.Delay(TimeSpan.FromSeconds(RepeatIntervalSeconds), cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }

    private void CancelHold()
    {
        if (null == _holdCts)
        {
            return;
        }

        _holdCts.Cancel();
        _holdCts.Dispose();
        _holdCts = null;
    }
}