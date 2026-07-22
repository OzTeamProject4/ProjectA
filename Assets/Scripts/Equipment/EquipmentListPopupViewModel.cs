//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class EquipmentListPopupViewModel
//{
//    private readonly Inventory _inventory;
//    private readonly CharacterModel _characterModel;
//    private readonly EquipType _slotType;

//    private readonly List<EquipmentListItemViewModel> _items = new();

//    private EquipmentListItemViewModel _selectedItem;

//    public IReadOnlyList<EquipmentListItemViewModel> Items
//    {
//        get { return _items; }
//    }

//    public EquipType SlotType
//    {
//        get { return _slotType; }
//    }

//    public CharacterModel CharacterModel
//    {
//        get { return _characterModel; }
//    }

//    public event Action<EquipmentListItemViewModel> OnItemAdded;

//    public EquipmentListPopupViewModel(Inventory inventory, CharacterModel characterModel, EquipType slotType)
//    {
//        if (null == inventory || null == characterModel)
//        {
//            Debug.LogError("EquipmentListPopupViewModel: inventory 또는 characterModel 이 null 입니다.");
//            return;
//        }

//        _inventory = inventory;
//        _characterModel = characterModel;
//        _slotType = slotType;

//        foreach (EquipmentInstance instance in inventory.GetEquipmentByType(slotType))
//        {
//            _items.Add(new EquipmentListItemViewModel(characterModel, instance));
//        }
//    }

//    public void Initialize()
//    {
//        _inventory.OnEquipmentCreated += HandleEquipmentCreated;
//    }

//    public void Dispose()
//    {
//        _inventory.OnEquipmentCreated -= HandleEquipmentCreated;
//        _selectedItem = null;
//    }

//    public void SelectItem(EquipmentListItemViewModel item)
//    {
//        if (_selectedItem == item)
//        {
//            return;
//        }

//        ClearSelection();

//        _selectedItem = item;

//        if (null != _selectedItem)
//        {
//            _selectedItem.SetSelected(true);
//        }
//    }

//    public void ClearSelection()
//    {
//        if (null == _selectedItem)
//        {
//            return;
//        }

//        _selectedItem.SetSelected(false);
//        _selectedItem = null;
//    }

//    private void HandleEquipmentCreated(EquipmentInstance instance)
//    {
//        if (null == instance || instance.Type != _slotType)
//        {
//            return;
//        }

//        EquipmentListItemViewModel itemViewModel = new EquipmentListItemViewModel(_characterModel, instance);
//        _items.Add(itemViewModel);

//        OnItemAdded?.Invoke(itemViewModel);
//    }
//}