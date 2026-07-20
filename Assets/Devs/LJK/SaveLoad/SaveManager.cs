using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData
{
    public AudioSettingData AudioSettingData;
    public GrowthSaveData Growth;
}

public static class FileName
{
    public static string SaveFileName = "SaveData.json";
}

public static class Default
{
    public static float DefaultVolume = 0.5f;
}

public class SaveManager
{
    private readonly string _savePath;

    private readonly AudioSettingData _cachedAudioSettingData = new AudioSettingData();

    private IGameDataProvider _dataProvider;

    public SaveManager()
    {
        _savePath = Path.Combine(Application.persistentDataPath, FileName.SaveFileName);
    }

    public void RequestSaveGame(ModelContainer modelContainer)
    {
        SaveData saveData = CreateSaveData(modelContainer);

        string jsonString = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(_savePath, jsonString);
        Debug.Log("세이브 완료");
    }

    public ModelContainer RequestLoadGame()
    {
        if (!File.Exists(_savePath))
        {
            ModelContainer defaultModelContainer = CreateDefaultModelContainer();
            return defaultModelContainer;
        }

        string jsonString = File.ReadAllText(_savePath);
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(jsonString);

        ModelContainer saveModelContainer =  CreateModelContainer(saveData);
        return saveModelContainer;
    }

    private IGameDataProvider GetDataProvider()
    {
        if (_dataProvider == null)
        {
            _dataProvider = new GameDataProvider();
        }

        return _dataProvider;
    }

    #region Save
    private SaveData CreateSaveData(ModelContainer modelContainer)
    {
        UpdateCachedAudioSettingData(modelContainer);

        SaveData saveData = new SaveData();
        saveData.AudioSettingData = _cachedAudioSettingData;

        GrowthModel growthModel = modelContainer.GetModel<GrowthModel>();
        saveData.Growth = CreateGrowthSaveData(growthModel);

        return saveData;
    }

    private T GetModel<T>(ModelContainer modelContainer) where T : DataModel
    {
        T model = modelContainer.GetModel<T>();

        if (model == null)
        {
            Debug.LogError($"[{nameof(SaveManager)}:{nameof(GetModel)}] '{typeof(T).Name}' 을(를) 가져오지 못했습니다.");
        }

        return model;
    }

    private void UpdateCachedAudioSettingData(ModelContainer modelContainer)
    {
        AudioModel audioModel = GetModel<AudioModel>(modelContainer);

        _cachedAudioSettingData.BgmVolume = audioModel.BgmVolume;
        _cachedAudioSettingData.SfxVolume = audioModel.SfxVolume;
    }

    private GrowthSaveData CreateGrowthSaveData(GrowthModel growthModel)
    {
        GrowthSaveData growthSaveData = new GrowthSaveData();

        if (growthModel == null)
        {
            return growthSaveData;
        }

        growthSaveData.Gold = growthModel.Inventory.Gold;

        foreach (KeyValuePair<string, CharacterModel> pair in growthModel.GetAllCharacters())
        {
            CharacterModel characterModel = pair.Value;

            CharacterSaveData characterSaveData = new CharacterSaveData();

            characterSaveData.CharacterId = characterModel.CharacterId;
            characterSaveData.CurrentStar = characterModel.CurrentStar;
            characterSaveData.CurrentLevel = characterModel.CurrentLevel;
            characterSaveData.CurrentExp = characterModel.CurrentExp;
            characterSaveData.OwnedDuplicates = characterModel.OwnedDuplicates;

            growthSaveData.Characters.Add(characterSaveData);
        }

        foreach (EquipmentInstance instance in growthModel.Inventory.GetAllEquipment())
        {
            EquipmentSaveData equipmentSaveData = new EquipmentSaveData();

            equipmentSaveData.InstanceId = instance.InstanceId;
            equipmentSaveData.DataId = instance.Data.DataId;
            equipmentSaveData.EquippedBy = instance.EquippedBy;
            equipmentSaveData.RolledHp = instance.RolledStat.Hp;
            equipmentSaveData.RolledAtk = instance.RolledStat.Atk;
            equipmentSaveData.RolledDef = instance.RolledStat.Def;
            equipmentSaveData.RolledMoveSpeed = instance.RolledStat.MoveSpeed;

            growthSaveData.Equipments.Add(equipmentSaveData);
        }

        foreach (string dataId in growthModel.Inventory.GetAllItemDataIds())
        {
            growthSaveData.ItemCounts[dataId] = growthModel.Inventory.GetItemCount(dataId);
        }

        return growthSaveData;
    }

    #endregion

    #region Load
    private ModelContainer CreateModelContainer(SaveData saveData)
    {
        ModelContainer modelContainer = new ModelContainer();

        AudioModel audioModel = CreateAudioModel(saveData.AudioSettingData);
        modelContainer.SetModel(audioModel);

        GrowthModel growthModel = CreateGrowthModel(saveData.Growth);
        modelContainer.SetModel(growthModel);

        return modelContainer;
    }

    private ModelContainer CreateDefaultModelContainer()
    {
        ModelContainer modelContainer = new ModelContainer();

        AudioModel audioModel = CreateAudioModel();
        modelContainer.SetModel(audioModel);

        GrowthModel growthModel = CreateDefaultGrowthModel();
        modelContainer.SetModel(growthModel);

        return modelContainer;
    }

    private AudioModel CreateAudioModel(AudioSettingData audioSettingData = null)
    {
        if (audioSettingData == null)
        {
            audioSettingData = CreateDefaultAudioSettingData();
        }

        AudioModel audioModel = new AudioModel(audioSettingData);
        return audioModel;
    }

    private AudioSettingData CreateDefaultAudioSettingData()
    {
        AudioSettingData audioSettingData = new AudioSettingData();

        audioSettingData.BgmVolume = Default.DefaultVolume;
        audioSettingData.SfxVolume = Default.DefaultVolume;

        return audioSettingData;
    }

    private GrowthModel CreateGrowthModel(GrowthSaveData growthSaveData)
    {
        IGameDataProvider dataProvider = GetDataProvider();
        Inventory inventory = GameManager.Instance.Inventory;

        Dictionary<string, CharacterModel> characters = new Dictionary<string, CharacterModel>();

        if (growthSaveData == null)
        {
            return new GrowthModel(inventory, characters);
        }

        inventory.RestoreGold(growthSaveData.Gold);

        foreach (KeyValuePair<string, int> pair in growthSaveData.ItemCounts)
        {
            inventory.RestoreItemCount(pair.Key, pair.Value);
        }

        foreach (EquipmentSaveData equipmentSaveData in growthSaveData.Equipments)
        {
            EquipmentData equipmentData = dataProvider.GetEquipment(equipmentSaveData.DataId);

            if (equipmentData == null)
            {
                Debug.LogWarning($"[{nameof(SaveManager)}:{nameof(CreateGrowthModel)}] EquipmentData 를 찾을 수 없습니다. dataId={equipmentSaveData.DataId}");
                continue;
            }

            RolledStats rolledStat = new RolledStats
            {
                Hp = equipmentSaveData.RolledHp,
                Atk = equipmentSaveData.RolledAtk,
                Def = equipmentSaveData.RolledDef,
                MoveSpeed = equipmentSaveData.RolledMoveSpeed
            };

            inventory.RestoreEquipment(equipmentSaveData.InstanceId, equipmentData, rolledStat, equipmentSaveData.EquippedBy);
        }

        foreach (CharacterSaveData characterSaveData in growthSaveData.Characters)
        {
            CharacterModel characterModel = new CharacterModel(characterSaveData.CharacterId, characterSaveData.CurrentStar, dataProvider, inventory);
            characterModel.RestoreProgress(characterSaveData.CurrentStar, characterSaveData.CurrentLevel, characterSaveData.CurrentExp, characterSaveData.OwnedDuplicates);

            characters[characterSaveData.CharacterId] = characterModel;
        }

        return new GrowthModel(inventory, characters);
    }

    private GrowthModel CreateDefaultGrowthModel()
    {
        IGameDataProvider dataProvider = GetDataProvider();
        Inventory inventory = GameManager.Instance.Inventory;

        // TODO: 기획 확정 시 최초 지급 캐릭터/재화/아이템 값으로 교체
        inventory.RestoreGold(999999);

        foreach (string expItemDataId in dataProvider.GetAllExpItemIds())
        {
            inventory.RestoreItemCount(expItemDataId, 999);
        }

        // 임시 지급
        inventory.RestoreItemCount("Item_Mat_T1", 999);
        inventory.RestoreItemCount("Item_Mat_T2", 999);
        inventory.RestoreItemCount("Item_Mat_T3", 999);
        inventory.RestoreItemCount("Item_Mat_Signature", 999);

        Dictionary<string, CharacterModel> characters = new Dictionary<string, CharacterModel>();

        string[] defaultCharacterIds = new string[]
        {
            "Character_001",
            "Character_002",
            "Character_003"
        };

        foreach (string dataId in defaultCharacterIds)
        {
            CharacterData characterData = dataProvider.GetStat(dataId);

            if (characterData == null)
            {
                Debug.LogWarning($"[{nameof(SaveManager)}:{nameof(CreateDefaultGrowthModel)}] CharacterData 를 찾을 수 없습니다. dataId={dataId}");
                continue;
            }

            CharacterModel characterModel = new CharacterModel(dataId, characterData.Star, dataProvider, inventory);
            characterModel.AddDuplicate(999);   // 임시 지급

            characters[dataId] = characterModel;
        }

        return new GrowthModel(inventory, characters);
    }

    #endregion
}
