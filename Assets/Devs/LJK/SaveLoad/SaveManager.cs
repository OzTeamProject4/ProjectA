using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData
{
    public AudioSettingData Audio;
}

public class SaveManager
{
    private readonly string NetworkPath;

    public SaveManager()
    {
        NetworkPath = Path.Combine(Application.persistentDataPath, "SaveData.json");
    }

    public void RequstSaveGame(ModelContainer modelContainer)
    {
        SaveData saveData = CreateSaveData(modelContainer);

        string jsonString = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(NetworkPath, jsonString);
    }

    private SaveData CreateSaveData(ModelContainer modelContainer)
    {
        SaveData data = new SaveData();

        AudioModel audioModel = modelContainer.GetModel<AudioModel>();

        data.Audio = CreateSetting(audioModel);

        return data;
    }

    private AudioSettingData CreateSetting(AudioModel audioModel)
    {
        AudioSettingData audioSettingData = new AudioSettingData();
        audioSettingData.BgmVolume = audioModel.BgmVolume;
        audioSettingData.SfxVolume = audioModel.SfxVolume;
        return audioSettingData;
    }

    public ModelContainer RequstLoadGame()
    {
        if (File.Exists(NetworkPath))
        {
            string jsonString = File.ReadAllText(NetworkPath);
            SaveData data = JsonConvert.DeserializeObject<SaveData>(jsonString);
            ModelContainer modelContainer = CreateModelC(data);
            return modelContainer;
        }
        else
        {
            return DefaultLoad();
        }
    }

    private ModelContainer CreateModelC(SaveData data)
    {
        ModelContainer modelContainer = new ModelContainer();

        AudioSettingData audioSettingData = new AudioSettingData();
        audioSettingData.BgmVolume = data.Audio.BgmVolume;
        audioSettingData.SfxVolume = data.Audio.BgmVolume;
        AudioModel model = new AudioModel(audioSettingData);
        modelContainer.SetModel(model);
        
        return modelContainer;
    }

    private ModelContainer DefaultLoad()
    {
        ModelContainer modelContainer = new ModelContainer();
        AudioSettingData audioSettingData = new AudioSettingData();
        audioSettingData.BgmVolume = 0.5f;
        audioSettingData.SfxVolume = 0.5f;
        AudioModel model = new AudioModel(audioSettingData);
        modelContainer.SetModel(model);
        return modelContainer;
    }
}
