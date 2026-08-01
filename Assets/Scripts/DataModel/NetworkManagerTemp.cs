using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerTemp : MonoBehaviour
{
    public static NetworkManagerTemp Instance { get; private set; }

    // 인스턴스 ID 발급
    private int _equipmentInstanceCounter;

    private StudentListModel _studentListModel;
    private InventoryModel _inventoryModel;
    private EquipmentCraftListModel _equipmentCraftListModel;
    private PlayerProfileModel _playerProfileModel;
    private StageClearModel _stageClearModel;

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

    public PlayerProfileModel PlayerProfileModel
    {
        get
        {
            if (_playerProfileModel == null)
            {
                _playerProfileModel = CreatePlayerProfileModel();
            }

            return _playerProfileModel;
        }
    }

    public StageClearModel StageClearModel
    {
        get
        {
            if (_stageClearModel == null)
            {
                _stageClearModel = CreateStageClearModel();
            }

            return _stageClearModel;
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

    private static readonly string[] DefaultStudentDataIds =
    {
        "Character_001",
        "Character_002",
        "Character_003",
        "Character_004",
        "Character_005"
    };

    private StudentListModel CreateStudentListModel()
    {
        List<StudentModel> studentSaveTemp = new List<StudentModel>();

        //TODO 저장 데이터가 붙으면 보유 학생 목록을 서버에서 받아오기
        foreach (string studentDataId in DefaultStudentDataIds)
        {
            if (!GameManager.Instance.DataManager.TryGetData(studentDataId, out StudentData studentData))
            {
                Debug.LogError($"'{studentDataId}' StudentData를 찾을 수 없습니다.");
                continue;
            }

            studentSaveTemp.Add(new StudentModel(studentData));
        }

        StudentListModel studentListModel = new StudentListModel(studentSaveTemp);

        return studentListModel;
    }
    private InventoryModel CreateInventoryModel()
    {
        InventoryModel inventoryModel = new InventoryModel();

        AddMaterial(inventoryModel, CurrencyItemId.Gold, 100000);
        AddMaterial(inventoryModel, CurrencyItemId.Crystal, 1200);

        AddMaterial(inventoryModel, "Item_ExpBook_Small", 99);
        AddMaterial(inventoryModel, "Item_ExpBook_Medium", 99);
        AddMaterial(inventoryModel, "Item_ExpBook_Large", 99);

        AddMaterial(inventoryModel, "Item_Mat_Shard_001", 20);
        AddMaterial(inventoryModel, "Item_Mat_Shard_002", 20);
        AddMaterial(inventoryModel, "Item_Mat_Shard_003", 20);
        AddMaterial(inventoryModel, "Item_Mat_Shard_004", 20);
        AddMaterial(inventoryModel, "Item_Mat_Shard_005", 20);

        // 장비 제작 재료
        AddMaterial(inventoryModel, "Item_Mat_T1", 50);
        AddMaterial(inventoryModel, "Item_Mat_T2", 50);
        AddMaterial(inventoryModel, "Item_Mat_T3", 50);

        AddEquipment(inventoryModel, "Item_Equipment_01");
        AddEquipment(inventoryModel, "Item_Equipment_01");
        AddEquipment(inventoryModel, "Item_Equipment_02");
        AddEquipment(inventoryModel, "Item_Equipment_04");
        AddEquipment(inventoryModel, "Item_Equipment_07");
        AddEquipment(inventoryModel, "Item_Equipment_10");
        AddEquipment(inventoryModel, "Item_Equipment_13");

        return inventoryModel;
    }

    public string CreateEquipmentInstanceId()
    {
        _equipmentInstanceCounter++;

        return $"Equip_{_equipmentInstanceCounter:D4}";
    }

    private void AddEquipment(InventoryModel inventoryModel, string itemDataId)
    {
        if (!TryGetItemData(itemDataId, out ItemData itemData))
        {
            return;
        }

        inventoryModel.AddEquipment(new EquipmentModel(CreateEquipmentInstanceId(), itemData));
    }

    private static void AddMaterial(InventoryModel inventoryModel, string itemDataId, int count)
    {
        if (!TryGetItemData(itemDataId, out ItemData itemData))
        {
            return;
        }

        inventoryModel.AddExpItem(new MaterialModel(itemData, count));
    }

    private static bool TryGetItemData(string itemDataId, out ItemData itemData)
    {
        if (GameManager.Instance.DataManager.TryGetData(itemDataId, out itemData))
        {
            return true;
        }

        Debug.LogError($"'{itemDataId}' ItemData를 찾을 수 없습니다.");
        return false;
    }

    private EquipmentCraftListModel CreateEquipmentCraftListModel()
    {
        List<EquipmentCraftModel> equipmentCraftModels = new List<EquipmentCraftModel>();

        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, ItemData> itemDataTable))
        {
            Debug.LogError("ItemData 테이블을 찾을 수 없습니다.");
            return new EquipmentCraftListModel(equipmentCraftModels);
        }

        foreach (ItemData itemData in itemDataTable.Values)
        {
            if (itemData.ItemType != ItemType.Equipment)
            {
                continue;
            }

            equipmentCraftModels.Add(new EquipmentCraftModel(itemData));
        }

        return new EquipmentCraftListModel(equipmentCraftModels);
    }

    private StageClearModel CreateStageClearModel()
    {
        StageClearModel stageClearModel = new StageClearModel(Array.Empty<string>());

        return stageClearModel;
    }

    private PlayerProfileModel CreatePlayerProfileModel()
    {
        PlayerProfileSaveTemp.LoadAndUpdate(DateTime.Now, out DateTime accountCreatedAt, out DateTime lastConnectAt, out int totalLoginDays);

        PlayerProfileModel playerProfileModel = new PlayerProfileModel(
            "선생님",
            12,
            "신입 교사",
            accountCreatedAt,
            lastConnectAt,
            totalLoginDays,
            "잘 부탁드립니다!",
            1,
            1,
            "Character_001");

        return playerProfileModel;
    }

    public bool TryGetStudentStats(string studentDataId, out StatData statData)
    {
        statData = default;

        if (string.IsNullOrEmpty(studentDataId))
        {
            Debug.LogError("[NetworkManagerTemp] studentDataId 가 비어 있습니다.");
            return false;
        }

        StudentModel studentModel = StudentListModel.GetCharacter(studentDataId);

        if (studentModel == null)
        {
            Debug.LogError($"[NetworkManagerTemp] 보유하지 않은 학생입니다. DataId={studentDataId}");
            return false;
        }

        statData = new StatData(studentModel.TotalHp, studentModel.TotalAttack, studentModel.TotalDefense, studentModel.TotalMoveSpeed);

        return true;
    }
}
