using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

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
    private StudentListModel _studentListModel;
    //private InventoryModel _inventoryModel;
    //private CraftModel _craftModel;

    public StudentListModel StudentListModel
    {
        get
        {
            if (_studentListModel == null)
            {
                _studentListModel = CreateStudentListModel();
            }

            return _studentListModel;
        }
    }

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

    //    GameManager.Instance.DataManager.TryGetData("Item_ExpBook_Small", out ItemData itemData);

    //    inventoryModel.AddItem(itemData.DataId, itemData.SpritePath, 30, EquipType.Signature, 1, ItemType.ExpBook, itemData.Value);
    //    return inventoryModel;
    //}

    private StudentListModel CreateStudentListModel()
    {
        List<StudentModel> studentSaveTemp = new List<StudentModel>
        {
            new StudentModel(new CharacterData("3", "3", 3, "Test")),
            new StudentModel(new CharacterData("4", "4", 4, "Test"))
        };

        StudentListModel studentListModel = new StudentListModel(studentSaveTemp);

        return studentListModel;
    }

    //public CharacterModel GetcharacterModel(string id)
    //{
    //    if (!charModel.TryGetValue(id, out CharacterModel characterModel))
    //    {
    //        characterModel = new CharacterModel(new CharacterData(id, "1", 3, "Test"));
    //        charModel[id] = characterModel;
    //    }

    //    return characterModel;
    //}

    //public CraftModel GetCraftModel()
    //{
    //    if (_craftModel == null)
    //    {
    //        _craftModel = CreateCraftModel();
    //    }

    //    return _craftModel;
    //}

    private CraftModel CreateCraftModel()
    {
        if (!GameManager.Instance.DataManager.TryGetDataTable<EquipmentData>(out var dataTable))
        {
            Debug.LogError("dasdad");
            return null;
        }

        CraftModel craftModel = new CraftModel();

        foreach (EquipmentData item in dataTable.Values)
        {
            CraftItemModel craftItemModel = new CraftItemModel(item);
            craftModel.AddCraftItem(craftItemModel);
        }

        return craftModel;
    }
}
