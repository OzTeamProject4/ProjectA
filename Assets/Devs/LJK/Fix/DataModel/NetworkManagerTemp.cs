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
    private StudentListModel _studentListModel;
    private InventoryModel _inventoryModel;
    private EquipmentCraftListModel _equipmentCraftListModel;

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

    public EquipmentCraftListModel EquipmentCraftListModel
    {
        get
        {
            if (_equipmentCraftListModel == null)
            {
                _equipmentCraftListModel = CreateEquipmentCraftListModel();
            }

            return _equipmentCraftListModel;
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

        MaterialModel materialModel1 = new MaterialModel(new ItemData { Name = "초급 경험치 책", Description = "학생에게 사용하면 경험치를 획득합니다.", IconKey = "Sprites/Items[Items_ExpBook_1]", ItemType = ItemType.Material, DataId = "1" },1, 10, 10);
        MaterialModel materialModel2 = new MaterialModel(new ItemData { Name = "초급 경험치 책", Description = "학생에게 사용하면 경험치를 획득합니다.", IconKey = "Sprites/Items[Items_ExpBook_1]", ItemType = ItemType.Material, DataId = "2" },2, 20, 10);
        MaterialModel materialModel3 = new MaterialModel(new ItemData { Name = "초급 경험치 책", Description = "학생에게 사용하면 경험치를 획득합니다.", IconKey = "Sprites/Items[Items_ExpBook_1]", ItemType = ItemType.Material, DataId = "3" },3, 30, 10);

        inventoryModel.AddExpItem(materialModel1);
        inventoryModel.AddExpItem(materialModel2);
        inventoryModel.AddExpItem(materialModel3);

        return inventoryModel;
    }

    //TODO 데이터에 맞게 구조 수정
    private EquipmentCraftListModel CreateEquipmentCraftListModel()
    {
        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, EquipmentData> equipmentDataTable))
        {
            Debug.LogError("dasdad");
            return null;
        }

        List<EquipmentCraftModel> equipmentCraftModels = new List<EquipmentCraftModel>();

        //foreach (EquipmentData equipmentData in equipmentDataTable.Values)
        //{
        //    EquipmentCraftModel equipmentCraftModel = new EquipmentCraftModel(equipmentData);
        //    equipmentCraftModels.Add(equipmentCraftModel);
        //}

        EquipmentCraftListModel equipmentCraftListModel = new EquipmentCraftListModel(equipmentCraftModels);

        return equipmentCraftListModel;
    }
}
