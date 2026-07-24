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
    private InventoryModel _inventoryModel;
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

    public InventoryModel InventoryModel
    {
        get
        {
            if (_inventoryModel == null)
            {
                _inventoryModel = CreateInventoryModel();
            }

            return _inventoryModel;
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

    private StudentListModel CreateStudentListModel()
    {
        List<StudentModel> studentSaveTemp = new List<StudentModel>
        {
            new StudentModel(new StudentData
            {
                DataId = "Student_001",
                Name = "Elena",
                Star = 1,
                FullBodyKey = "Test",
                PortraitKey = "Test",
                BaseHp = 100,
                BaseAttack = 20,
                BaseDefense = 10,
                BaseMoveSpeed = 5,
                ElementType = ElementType.Fire
            }),
            new StudentModel(new StudentData
            {
                DataId = "Student_002",
                Name = "Seria",
                Star = 2,
                FullBodyKey = "Test",
                PortraitKey = "Test",
                BaseHp = 150,
                BaseAttack = 15,
                BaseDefense = 20,
                BaseMoveSpeed = 4,
                ElementType = ElementType.Water
            }),
            new StudentModel(new StudentData
            {
                DataId = "Student_003",
                Name = "Luna",
                Star = 3,
                FullBodyKey = "Test",
                PortraitKey = "Test",
                BaseHp = 80,
                BaseAttack = 35,
                BaseDefense = 5,
                BaseMoveSpeed = 7,
                ElementType = ElementType.Grass
            })
        };

        StudentListModel studentListModel = new StudentListModel(studentSaveTemp);

        return studentListModel;
    }
    private InventoryModel CreateInventoryModel()
    {
        InventoryModel inventoryModel = new InventoryModel();
        return inventoryModel;
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
