//using System;

//public class ExpItemSlotViewModel
//{
//    private readonly CharacterModel _character;
//    private readonly Inventory _inventory;
//    private readonly ItemData _itemData;

//    public string DataId
//    {
//        get { return _itemData.DataId; }
//    }

//    public string Name
//    {
//        get { return _itemData.Name; }
//    }

//    public string SpritePath
//    {
//        get { return _itemData.SpritePath; }
//    }

//    public int OwnedCount
//    {
//        get { return _inventory.GetItemCount(DataId); }
//    }

//    public bool IsUsable
//    {
//        get { return _character.CanUseExpItem(DataId); }
//    }

//    public event Action OnChanged;

//    public ExpItemSlotViewModel(CharacterModel character, Inventory inventory, ItemData itemData)
//    {
//        _character = character;
//        _inventory = inventory;
//        _itemData = itemData;
//    }

//    public void Initialize()
//    {
//        _inventory.OnItemChanged += HandleChanged;
//        _character.OnLevelChanged += HandleChanged;
//        _character.OnStarChanged += HandleChanged;
//    }

//    public void Dispose()
//    {
//        _inventory.OnItemChanged -= HandleChanged;
//        _character.OnLevelChanged -= HandleChanged;
//        _character.OnStarChanged -= HandleChanged;
//    }

//    private void HandleChanged()
//    {
//        OnChanged?.Invoke();
//    }
//}
