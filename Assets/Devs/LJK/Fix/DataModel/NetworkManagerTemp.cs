using System.Collections.Generic;
using UnityEngine;

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
                DataId = "Character_001",
                Name = "루미",
                Star = 3,
                FullBodyKey = "StandImage/Lumi",
                PortraitKey = "Icon/Lumi",
                BaseHp = 100,
                BaseAttack = 20,
                BaseDefense = 10,
                BaseMoveSpeed = 3.5f,
                HpGrow = 18.6f,
                AtkGrow = 2.7f,
                DefGrow = 1.55f,
                MoveSpeedGrow = 0.017f,
                ElementType = ElementType.Fire
            }),
            new StudentModel(new StudentData
            {
                DataId = "Character_002",
                Name = "네리",
                Star = 3,
                FullBodyKey = "StandImage/Neri",
                PortraitKey = "Icon/Neri",
                BaseHp = 140,
                BaseAttack = 15,
                BaseDefense = 16,
                BaseMoveSpeed = 3.2f,
                HpGrow = 24f,
                AtkGrow = 1.9f,
                DefGrow = 2.4f,
                MoveSpeedGrow = 0.012f,
                ElementType = ElementType.Water
            }),
            new StudentModel(new StudentData
            {
                DataId = "Character_003",
                Name = "카이",
                Star = 3,
                FullBodyKey = "StandImage/Kai",
                PortraitKey = "Icon/Kai",
                BaseHp = 80,
                BaseAttack = 26,
                BaseDefense = 7,
                BaseMoveSpeed = 4f,
                HpGrow = 14.4f,
                AtkGrow = 3.6f,
                DefGrow = 1.05f,
                MoveSpeedGrow = 0.02f,
                ElementType = ElementType.Grass
            }),
            new StudentModel(new StudentData
            {
                DataId = "Character_004",
                Name = "빛나",
                Star = 4,
                FullBodyKey = "StandImage/Bitna",
                PortraitKey = "Icon/Bitna",
                BaseHp = 95,
                BaseAttack = 17,
                BaseDefense = 9,
                BaseMoveSpeed = 4.2f,
                HpGrow = 17.1f,
                AtkGrow = 2.2f,
                DefGrow = 1.35f,
                MoveSpeedGrow = 0.022f,
                ElementType = ElementType.Normal
            }),
            new StudentModel(new StudentData
            {
                DataId = "Character_005",
                Name = "유이",
                Star = 4,
                FullBodyKey = "StandImage/Yui",
                PortraitKey = "Icon/Yui",
                BaseHp = 160,
                BaseAttack = 22,
                BaseDefense = 13,
                BaseMoveSpeed = 3f,
                HpGrow = 27.2f,
                AtkGrow = 2.9f,
                DefGrow = 1.95f,
                MoveSpeedGrow = 0.011f,
                ElementType = ElementType.Normal
            })
        };

        StudentListModel studentListModel = new StudentListModel(studentSaveTemp);

        return studentListModel;
    }
    private InventoryModel CreateInventoryModel()
    {
        InventoryModel inventoryModel = new InventoryModel();

        MaterialModel materialModel1 = new MaterialModel(new ItemData
        {
            DataId = "Item_ExpBook_Small",
            TypeDataId = "Currency_ExpBook_Small",
            ItemType = ItemType.Currency,
            Name = "경험치북(소)",
            Description = "학생에게 사용하면 경험치를 획득합니다.",
            IconKey = "Sprites/Items[Items_ExpBook_1]"
        }, 10);

        MaterialModel materialModel2 = new MaterialModel(new ItemData
        {
            DataId = "Item_ExpBook_Medium",
            TypeDataId = "Currency_ExpBook_Medium",
            ItemType = ItemType.Currency,
            Name = "경험치북(중)",
            Description = "학생에게 사용하면 경험치를 획득합니다.",
            IconKey = "Sprites/Items[Items_ExpBook_2]"
        }, 10);

        MaterialModel materialModel3 = new MaterialModel(new ItemData
        {
            DataId = "Item_ExpBook_Large",
            TypeDataId = "Currency_ExpBook_Large",
            ItemType = ItemType.Currency,
            Name = "경험치북(대)",
            Description = "학생에게 사용하면 경험치를 획득합니다.",
            IconKey = "Sprites/Items[Items_ExpBook_3]"
        }, 10);

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
