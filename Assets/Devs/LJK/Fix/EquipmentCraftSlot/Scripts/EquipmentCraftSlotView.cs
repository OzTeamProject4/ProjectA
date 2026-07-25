using Cysharp.Threading.Tasks;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentCraftSlotView : MonoBehaviour
{
    [SerializeField] private RectTransform _root;
    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private Image _itemIconImage;

    //리스트 아이템으로 변경
    [Header("Gold")]
    [SerializeField] private GameObject _goldObject;
    [SerializeField] private Image _goldIconImage;
    [SerializeField] private TMP_Text _goldAmountText;
    //리스트 아이템으로 변경
    [Header("Material 1")]
    [SerializeField] private GameObject _material1Object;
    [SerializeField] private Image _material1IconImage;
    [SerializeField] private TMP_Text _material1TierText;
    [SerializeField] private TMP_Text _material1CountText;
    //리스트 아이템으로 변경
    [Header("Material 2")]
    [SerializeField] private GameObject _material2Object;
    [SerializeField] private Image _material2IconImage;
    [SerializeField] private TMP_Text _material2TierText;
    [SerializeField] private TMP_Text _material2CountText;

    [SerializeField] private Button _iconButton;
    [SerializeField] private Button _craftButton;

    private EquipmentCraftSlotViewModel _equipmentCraftSlotViewModel;
    
    private CancellationTokenSource _disableCts;

    private void Awake()
    {
        _equipmentCraftSlotViewModel = new EquipmentCraftSlotViewModel();
    }

    private void OnEnable()
    {
        _disableCts = new CancellationTokenSource();

        _equipmentCraftSlotViewModel.OnPropertyChange += OnPropertyChanged;

        _iconButton.onClick.AddListener(OpenCraftItemInfoPopup);
        _craftButton.onClick.AddListener(OnButtonCLick);
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
        RefreshIconsImage();
        RefreshMaterialTexts();
        RefreshCraftButton();
        RefreshMaterialInventory();
    }

    private void OnPropertyChanged(object sender, string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_equipmentCraftSlotViewModel.RequiredItems):
                UpdateRequiredItemCount(sender);
                break;
        }
    }

    private void UpdateRequiredItemCount(object sender)
    {
        for (int index = 0; index < _equipmentCraftSlotViewModel.RequiredItems.Count; index++)
        {
            if (ReferenceEquals(sender, _equipmentCraftSlotViewModel.RequiredItems[index]))
            { 
                //TODO 리스트 인덱스 텍스트 변경
            }
        }
    }

    private void RefreshNameText()
    {
        _itemNameText.text = _equipmentCraftSlotViewModel.Name;
    }
   
    //TODO 리스트로 변경하면 아이콘 이미지 변경 로직
    private async UniTask UpdateIconImageAsync()
    {
        string iconKey = _equipmentCraftSlotViewModel.IconKey;

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

    //TODO 요구 재료 확인
    private void RefreshMaterialTexts()
    {
        //for (int index = 0; index < quipmentCraftSlotViewModel.RequiredItemIds.Count; index++)
        //{
        //    string requiredItemId = quipmentCraftSlotViewModel.RequiredItemIds[index];

        //    if (!GameManager.Instance.DataManager.TryGetData(requiredItemId, out ItemData itemData))
        //    {
        //        Debug.LogError("");
        //        return;
        //    }

        //    int tier = itemData.Tier;

        //    if (index == 0)
        //    {
        //        _mat1TierText.text = tier > 0 ? $"T{tier}" : "-";
        //    }
        //    else
        //    {
        //        _mat2TierText.text = tier > 0 ? $"T{tier}" : "-";
        //    }
        //}
    }

    //TODO 갯수만 변경
    //TODO 골드 택스트 변경
    private void RefreshMaterialInventory()
    {
        //for (int index = 0; index < _equipmentCraftSlotViewModel.RequiredItemIds.Count; index++)
        //{
        //    string requiredItemId = _equipmentCraftSlotViewModel.RequiredItemIds[index];
        //    int requiredItemCount = _equipmentCraftSlotViewModel.RequiredItemCounts[index];

        //    if (!_equipmentCraftSlotViewModel.ItemLists.TryGetValue(requiredItemId, out ItemModel itemModel))
        //    {
        //        HasItem = false;
        //    }

        //    if (index == 0)
        //    {
        //        _material1CountText.text = $"{owned}/{requiredItemCount}";
        //    }
        //    else
        //    {
        //        _material2CountText.text = $"{owned}/{requiredItemCount}";
        //    }
        //}
    }

    //TODO 기능 구현
    private void RefreshCraftButton()
    {
        //TODO 조건문 확인
        if (_equipmentCraftSlotViewModel.RequiredItemIds.Count != _equipmentCraftSlotViewModel.RequiredItemCounts.Count)
        {
            Debug.LogError(" ");
            return;
        }

        //for (int index = 0; index < _craftListItemViewModel.RequiredItemIds.Count; index++)
        //{
        //    string requiredItemId = _craftListItemViewModel.RequiredItemIds[index];
        //    int requiredItemCount = _craftListItemViewModel.RequiredItemCounts[index];

        //    if(!_craftListItemViewModel.ItemLists.TryGetValue(requiredItemId, out ItemModel itemModel))
        //    {
        //        _craftButton.interactable = false;
        //        return;
        //    }

        //    if(itemModel.Count < requiredItemCount)
        //    {
        //        _craftButton.interactable = false;
        //        return;
        //    }
        //}

        _craftButton.interactable = true;
    }

    private void RefreshIconsImage()
    {
        LoadSpriteAsync(_itemIconImage, _equipmentCraftSlotViewModel.IconKey).Forget();

        //TODO 리스트로 만들어서 초기화후 Add
        _material1IconImage.gameObject.SetActive(false);
        _material2IconImage.gameObject.SetActive(false);

        for (int index = 0; index < _equipmentCraftSlotViewModel.RequiredItemIds.Count; index++)
        {
            string requiredItemId = _equipmentCraftSlotViewModel.RequiredItemIds[index];

            if (!GameManager.Instance.DataManager.TryGetData(requiredItemId, out ItemData itemData))
            {
                Debug.LogError("");
                return;
            }

            //TODO  리스트로 만들어서 초기화후 Add
            if (index == 0)
            {
                LoadSpriteAsync(_material1IconImage, itemData.IconKey).Forget();
                _material1IconImage.gameObject.SetActive(true);
            }
            else
            {
                LoadSpriteAsync(_material2IconImage, itemData.IconKey).Forget();

                _material1IconImage.gameObject.SetActive(true);
            }
        }
    }

    private async UniTask LoadSpriteAsync(Image image, string spritePath)
    {
        Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(spritePath, destroyCancellationToken);

        if (null == sprite)
        {
            return;
        }

        image.sprite = sprite;
    }

    private void OnButtonCLick()
    {
        _equipmentCraftSlotViewModel.RequestCraftItem();
    }

    private void OpenCraftItemInfoPopup()
    {
        Vector3[] itemCorners = new Vector3[4];
        _root.GetWorldCorners(itemCorners);
        Vector3 itemBottomCenter = itemCorners[1];

        GameManager.Instance.UIManager.OpenCraftItemInfoPopupAsync(_equipmentCraftSlotViewModel.DataId, itemBottomCenter).Forget();
    }
}