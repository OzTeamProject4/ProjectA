using System.Collections.Generic;
using UnityEngine;

public class ItemDataTemp
{
    public string IconPath;

    public ItemDataTemp(string path)
    {
        IconPath = path;
    }
}

public class NetworkManagerTemp : MonoBehaviour
{
    public static NetworkManagerTemp Instance { get; private set; }

    private Dictionary<string, CharacterModel> charModel = new Dictionary<string, CharacterModel>();
    //private InventoryModel _inventoryModel;
    private CharacterListModel _characterListModel;
    //private CraftModel _craftModel;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
    }

    //public InventoryModel GetInventoryModel()
    //{
    //    if (_inventoryModel == null)
    //    {
    //        _inventoryModel = CreatePlayerViewModel();
    //    }

    //    return _inventoryModel;
    //}

    //private InventoryModel CreatePlayerViewModel()
    //{
    //    InventoryModel inventoryModel = new InventoryModel();

    //    return inventoryModel;
    //}

    public CharacterListModel GetcharacterListModel()
    {
        if (_characterListModel == null)
        {
            _characterListModel = CreatecharacterListModel();
        }

        return _characterListModel;
    }

    private CharacterListModel CreatecharacterListModel()
    {
        List<CharacterModel> characterModels = new List<CharacterModel>();

        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, CharacterData> dataTable))
        {
            Debug.LogError("[NetworkManagerTemp] CharacterData 테이블을 가져오지 못했습니다.");
            return new CharacterListModel(characterModels);
        }

        foreach (CharacterData characterData in dataTable.Values)
        {
            if (null == characterData)
            {
                continue;
            }

            characterModels.Add(new CharacterModel(characterData));
        }

        return new CharacterListModel(characterModels);
    }

    public CharacterModel GetcharacterModel(string id)
    {
        if (!charModel.TryGetValue(id, out CharacterModel characterModel))
        {
            characterModel = new CharacterModel(new CharacterData { DataId = id, Name = "1", Star = 3, Description = "Test" });
            charModel[id] = characterModel;
        }

        return characterModel;
    }

    //public CraftModel GetCraftModel()
    //{
    //    if (_craftModel == null)
    //    {
    //        _craftModel = CreateCraftModel();
    //    }

    //    return _craftModel;
    //}

    //private CraftModel CreateCraftModel()
    //{
    //    if (!GameManager.Instance.DataManager.TryGetDataTable<EquipmentData>(out var dataTable))
    //    {
    //        Debug.LogError("dasdad");
    //        return null;
    //    }

    //    CraftModel craftModel = new CraftModel();

    //    foreach (EquipmentData item in dataTable.Values)
    //    {
    //        CraftItemModel craftItemModel = new CraftItemModel(item);
    //        craftModel.AddCraftItem(craftItemModel);
    //    }

    //    return craftModel;
    //}
}