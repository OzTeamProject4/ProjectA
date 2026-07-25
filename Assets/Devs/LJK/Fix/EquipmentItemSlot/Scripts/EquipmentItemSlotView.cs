using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentItemSlotView : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransfrom;
    [SerializeField] private Button _slotButton;
    [SerializeField] private Image _itemIconImage;
    [SerializeField] private Image _equippedCharacterIconImage;
    [SerializeField] private GameObject _selector;
    [SerializeField] private TMP_Text _equippedText;

    private EquipmentItemSlotViewModel _equipmentItemSlotViewModel;

    public event Action<EquipmentModel, RectTransform> OnSlotClicked;

    private CancellationTokenSource _disableCts;

    private void Awake()
    {
        //TODO 추후 필드를 사용하면 추가
        UnityUtil.ValidateReference(_itemIconImage, nameof(EquipmentItemSlotView), nameof(_itemIconImage));

        _equipmentItemSlotViewModel = new EquipmentItemSlotViewModel();
    }

    public void OnEnable()
    {
        _disableCts = new CancellationTokenSource();

        _slotButton.onClick.AddListener(HandleSlotClicked);
    }

    public void OnDisable()
    {
        if (_disableCts == null)
        {
            _disableCts.Cancel();
            _disableCts.Dispose();
            _disableCts = null;
        }

        _slotButton.onClick.RemoveAllListeners();
    }

    public void SetModel(EquipmentModel equipmentModel)
    {
        _equipmentItemSlotViewModel.SetModel(equipmentModel);
       
        Refresh();
    }

    private void Refresh()
    {
        RefreshIconImageAsync().Forget();

        _equipmentItemSlotViewModel.Refresh();
    }

    //TODO 기능 추가
    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            //case nameof(_equipmentSlotViewModel.Name):
            //    break;
            //case nameof(_equipmentSlotViewModel.Count):
            //    break;
            //case nameof(_equipmentSlotViewModel.IconKey):
            //    break;
        }
    }

    private async UniTask RefreshIconImageAsync()
    {
        string iconKey = _equipmentItemSlotViewModel.IconKey;

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

    //TODO 기능 추가
    private async UniTaskVoid UpdateEquippedCharacterIconAsync()
    {
        //if (null == _equippedCharacterIconImage)
        //{
        //    return;
        //}

        //string iconPath = _equipmentSlotViewModel.EquippedCharacterIconPath;

        //if (string.IsNullOrEmpty(iconPath))
        //{
        //    _equippedCharacterIconImage.enabled = false;
        //    return;
        //}

        //try
        //{
        //    Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(iconPath, destroyCancellationToken);

        //    if (null == sprite)
        //    {
        //        _equippedCharacterIconImage.enabled = false;
        //        return;
        //    }

        //    _equippedCharacterIconImage.enabled = true;
        //    _equippedCharacterIconImage.sprite = sprite;
        //}
        //catch (OperationCanceledException)
        //{
        //    // 오브젝트 파괴로 취소됨, 무시
        //}
        //catch (Exception exception)
        //{
        //    Debug.LogWarning($"[EquipmentListItemView] 장착 캐릭터 초상화 로드 실패. iconPath={iconPath}\n{exception}");
        //}
    }

    private void HandleSlotClicked()
    {
        if (OnSlotClicked == null)
        {
            return;
        }

        OnSlotClicked.Invoke(_equipmentItemSlotViewModel.EquipmentModel, _rectTransfrom);
    }
}