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
    private SaveManager _saveManager;

    public ModelContainer ModelContainer { get; private set; }

    public async override UniTask InitializeAsync()
    {
        if (_saveManager == null)
        {
            _saveManager = new SaveManager();
        }

        LoadGame();
        await UniTask.CompletedTask;
    }

    public void SaveGame()
    {
        _saveManager.RequstSaveGame(ModelContainer);
    }

    public void LoadGame()
    {
        ModelContainer = _saveManager.RequstLoadGame();
    }
}
