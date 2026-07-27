using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentCraftSlotView : MonoBehaviour
{
    [SerializeField] private RectTransform _root;
    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private Image _itemIconImage;

    [Header("Material")]
    [SerializeField] private CraftMaterialItemView _materialItemPrefab;
    [SerializeField] private Transform _materialContent;

    [SerializeField] private Button _iconButton;
    [SerializeField] private Button _craftButton;

    private EquipmentCraftSlotViewModel _equipmentCraftSlotViewModel;

    private readonly List<CraftMaterialItemView> _spawnedMaterialList = new List<CraftMaterialItemView>();

    private CancellationTokenSource _disableCts;

    private void Awake()
    {
        UnityUtil.ValidateReference(_root, nameof(EquipmentCraftSlotView), nameof(_root));
        UnityUtil.ValidateReference(_itemNameText, nameof(EquipmentCraftSlotView), nameof(_itemNameText));
        UnityUtil.ValidateReference(_itemIconImage, nameof(EquipmentCraftSlotView), nameof(_itemIconImage));
        UnityUtil.ValidateReference(_materialItemPrefab, nameof(EquipmentCraftSlotView), nameof(_materialItemPrefab));
        UnityUtil.ValidateReference(_materialContent, nameof(EquipmentCraftSlotView), nameof(_materialContent));
        UnityUtil.ValidateReference(_iconButton, nameof(EquipmentCraftSlotView), nameof(_iconButton));
        UnityUtil.ValidateReference(_craftButton, nameof(EquipmentCraftSlotView), nameof(_craftButton));

        _equipmentCraftSlotViewModel = new EquipmentCraftSlotViewModel();
    }

    private void OnEnable()
    {
        _disableCts = new CancellationTokenSource();

        _equipmentCraftSlotViewModel.OnPropertyChange += OnPropertyChanged;

        _iconButton.onClick.AddListener(OpenCraftItemInfoPopup);
        _craftButton.onClick.AddListener(HandleSlotClicked);
    }

    private void OnDisable()
    {
        if (_disableCts != null)
        {
            _disableCts.Cancel();
            _disableCts.Dispose();
            _disableCts = null;
        }

        _equipmentCraftSlotViewModel.OnPropertyChange -= OnPropertyChanged;
     
        _iconButton.onClick.RemoveAllListeners();
        _craftButton.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        _equipmentCraftSlotViewModel.Dispose();
        _equipmentCraftSlotViewModel = null;
    }


    public void SetModel(EquipmentCraftModel equipmentCraftModel)
    {
        _equipmentCraftSlotViewModel.SetModel(equipmentCraftModel);
        Refresh();
    }

    private void Refresh()
    {
        RefreshNameText();
        SpriteLoader.LoadIntoAsync(_itemIconImage, _equipmentCraftSlotViewModel.IconKey, _disableCts.Token).Forget();
        RefreshMaterials();
        RefreshCraftButton();
    }

    private void OnPropertyChanged(object sender, string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_equipmentCraftSlotViewModel.RequiredItems):
                RefreshMaterials();
                RefreshCraftButton();
                break;
        }
    }

    private void RefreshNameText()
    {
        _itemNameText.text = _equipmentCraftSlotViewModel.Name;
    }
   
    //TODO 슬롯 오브젝트 풀 사용 생성
    private void RefreshMaterials()
    {
        ReleaseMaterials();

        IReadOnlyList<string> requiredItemIds = _equipmentCraftSlotViewModel.RequiredItemIds;
        IReadOnlyList<int> requiredItemCounts = _equipmentCraftSlotViewModel.RequiredItemCounts;

        for (int index = 0; index < requiredItemIds.Count; index++)
        {
            if (index >= requiredItemCounts.Count)
            {
                break;
            }

            string requiredItemId = requiredItemIds[index];

            CraftMaterialItemView materialItemView = Instantiate(_materialItemPrefab, _materialContent);

            materialItemView.UpdateView(
                _equipmentCraftSlotViewModel.GetItemTier(requiredItemId),
                _equipmentCraftSlotViewModel.GetOwnedItemCount(requiredItemId),
                requiredItemCounts[index]);

            materialItemView.UpdateIconAsync(_equipmentCraftSlotViewModel.GetItemIconKey(requiredItemId), _disableCts.Token).Forget();

            _spawnedMaterialList.Add(materialItemView);
        }
    }

    //TODO 슬롯 오브젝트 풀 사용 해제
    private void ReleaseMaterials()
    {
        foreach (CraftMaterialItemView materialItemView in _spawnedMaterialList)
        {
            if (materialItemView == null)
            {
                continue;
            }

            Destroy(materialItemView.gameObject);
        }

        _spawnedMaterialList.Clear();
    }

    private void RefreshCraftButton()
    {
        _craftButton.interactable = _equipmentCraftSlotViewModel.CanCraft;
    }

    private void HandleSlotClicked()
    {
        _equipmentCraftSlotViewModel.RequestCraftItem();
    }

    private void OpenCraftItemInfoPopup()
    {
        Vector3[] corners = new Vector3[4];
        _root.GetWorldCorners(corners);

        Vector3 bottomLeft = corners[0];
        Vector3 bottomRight = corners[3];

        Vector3 bottomCenter = (bottomLeft + bottomRight) * 0.5f;

        GameManager.Instance.UIManager.OpenCraftEquipmentInfoPopupAsync(_equipmentCraftSlotViewModel.EquipmentCraftModel, bottomCenter).Forget();
    }
}