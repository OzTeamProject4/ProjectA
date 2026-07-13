using System;
using System.Collections.Generic;

public class CraftListItemViewModel
{
    private readonly CraftingModel _crafting;
    private readonly Inventory _inventory;
    private readonly EquipmentData _data;

    public string DataId
    {
        get { return _data.DataId; }
    }

    public string Name
    {
        get { return _data.Name; }
    }

    public int RequiredGold
    {
        get { return _data.Gold; }
    }

    public bool CanCraft
    {
        get { return _crafting.CanCraft(_data); }
    }

    public IReadOnlyList<(string Name, string SpritePath, int Tier, int Owned, int Required)> GetMaterials()
    {
        List<(string, string, int, int, int)> result = new();

        if (!_data.TryGetRequiredMaterials(out (string ItemId, int Count)[] materials))
        {
            return result;
        }

        foreach ((string itemId, int count) in materials)
        {
            ItemData item = _crafting.GetMaterialData(itemId);
            string name = null == item ? itemId : item.Name;
            string spritePath = null == item ? null : item.SpritePath;
            int tier = Util.ParseMaterialTier(itemId);

            result.Add((name, spritePath, tier, _inventory.GetItemCount(itemId), count));
        }

        return result;
    }

    public event Action OnChanged;

    public CraftListItemViewModel(CraftingModel crafting, Inventory inventory, EquipmentData data)
    {
        _crafting = crafting;
        _inventory = inventory;
        _data = data;
    }

    public void Initialize()
    {
        _inventory.OnItemChanged += HandleChanged;
        _inventory.OnGoldChanged += HandleChanged;
    }

    public void Dispose()
    {
        _inventory.OnItemChanged -= HandleChanged;
        _inventory.OnGoldChanged -= HandleChanged;
    }

    public void CraftCommand()
    {
        _crafting.Craft(DataId);
    }

    private void HandleChanged()
    {
        OnChanged?.Invoke();
    }
}