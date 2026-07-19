using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ModelContainer
{
    private readonly Dictionary<Type, DataModel> _models = new Dictionary<Type, DataModel>();

    public void SetModel<T>(T model) where T : DataModel
    {
        if (model == null)
        {
            Debug.LogError($"[{nameof(ModelContainer)}:{nameof(SetModel)}] {typeof(T).Name}이 null입니다.");
            return;
        }

        if (!_models.TryAdd(typeof(T), model))
        {
            Debug.LogError($"[{nameof(ModelContainer)}:{nameof(SetModel)}] {typeof(T).Name}은 이미 등록되어 있습니다.");
        }
    }

    public T GetModel<T>() where T : DataModel
    {
        if (!_models.TryGetValue(typeof(T), out DataModel model))
        {
            Debug.LogError($"[{nameof(ModelContainer)}:{nameof(GetModel)}] '{typeof(T).Name}' 모델이 등록되어 있지 않습니다.");
            return null;
        }

        return (T)model;
    }
}

public class NetworkManager : BaseManager<NetworkManager>
{
    public ModelContainer ModelContainer { get; private set; } = new ModelContainer();

    public override UniTask InitializeAsync()
    {
        LoadModel();
        return UniTask.CompletedTask;
    }

    public void SaveModel()
    {
      
    }

    public void LoadModel()
    {
        AudioModel audioModel = new AudioModel(new AudioSettingData(1,1));

        ModelContainer.SetModel(audioModel);
    }
}
