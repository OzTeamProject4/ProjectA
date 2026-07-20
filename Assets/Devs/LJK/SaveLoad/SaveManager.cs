using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData
{
    public AudioSettingData AudioSettingData;
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

    public SaveManager()
    {
        _savePath = Path.Combine(Application.persistentDataPath, FileName.SaveFileName);
    }

    public void RequestSaveGame(ModelContainer modelContainer)
    {
        SaveData saveData = CreateSaveData(modelContainer);

        string jsonString = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(_savePath, jsonString);
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

    #region Save
    private SaveData CreateSaveData(ModelContainer modelContainer)
    {
        UpdateCachedAudioSettingData(modelContainer);

        SaveData saveData = new SaveData();
        saveData.AudioSettingData = _cachedAudioSettingData;

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
    #endregion

    #region Load
    private ModelContainer CreateModelContainer(SaveData saveData)
    {
        ModelContainer modelContainer = new ModelContainer();

        AudioModel audioModel = CreateAudioModel(saveData.AudioSettingData);

        modelContainer.SetModel(audioModel);

        return modelContainer;
    }

    private ModelContainer CreateDefaultModelContainer()
    {
        ModelContainer modelContainer = new ModelContainer();

        AudioModel audioModel = CreateAudioModel();

        modelContainer.SetModel(audioModel);

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
    #endregion
}
