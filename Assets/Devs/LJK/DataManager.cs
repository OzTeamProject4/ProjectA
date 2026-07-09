using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : BaseManager<DataManager>
{
    private readonly Dictionary<Type, object> _dataTables = new Dictionary<Type, object>();

    public override void Initialize()
    {
        //GameManager.Instance.ResourceManager.ReleaseAsset("Key");
        _dataTables.Clear();
    }

    public async UniTask PreloadDataAsync()
    {
        //await LoadDataAsync<T>("Key");
        await LoadDataAsync<CharacterStatData>(GrowthDataKeys.CharacterStat);
        await LoadDataAsync<CharacterGradeData>(GrowthDataKeys.CharacterGrade);
        await LoadDataAsync<LevelExpData>(GrowthDataKeys.LevelExp);
        await LoadDataAsync<ItemData>(GrowthDataKeys.Item);
    }

    public async UniTask LoadRuntimeDataAsync()
    {
        //await LoadDataAsync<T>("Key");
    }

    public async UniTask LoadDataAsync<T>(string key) where T : BaseData
    {
        Dictionary<string, T> loadedDataTable = await LoadDataTableAsync<T>(key);

        if (loadedDataTable == null)
        {
            Debug.LogError($"[DataManager:LoadDataAsync] '{key}' 데이터 테이블을 로드하지 못했습니다.");
            return;
        }

        _dataTables[typeof(T)] = loadedDataTable;
    }

    public bool TryGetDataTable<T>(out Dictionary<string, T> dataTable) where T : BaseData
    {
        dataTable = null;

        if (!_dataTables.TryGetValue(typeof(T), out object loadedDataTable))
        {
            Debug.LogError($"[DataManager:TryGetDataTable] '{typeof(T).Name}' 데이터 테이블이 로드되어 있지 않습니다.");
            return false;
        }

        if (loadedDataTable is not Dictionary<string, T> typeCastDataTable)
        {
            Debug.LogError($"[DataManager:TryGetDataTable] '{typeof(T).Name}' 로드된 데이터 테이블과 타입이 일치하지 않습니다.");
            return false;
        }

        dataTable = typeCastDataTable;
        return true;
    }

    public bool TryGetData<T>(string dataId, out T data) where T : BaseData
    {
        data = null;

        if (string.IsNullOrWhiteSpace(dataId))
        {
            Debug.LogError("[DataManager:TryGetData] 전달된 DataId가 null이거나 빈 문자열 또는 공백 문자열입니다.");
            return false;
        }

        if (!TryGetDataTable(out Dictionary<string, T> dataTable))
        {
            return false;
        }

        if (!dataTable.TryGetValue(dataId, out data))
        {
            Debug.LogError($"[DataManager:TryGetData] '{typeof(T).Name}' 데이터 테이블에서 DataId '{dataId}'를 찾을 수 없습니다.");
            return false;
        }

        return true;
    }

    private async UniTask<Dictionary<string, T>> LoadDataTableAsync<T>(string key) where T : BaseData
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError("[DataManager:LoadDataTableAsync] 전달된 key가 null이거나 빈 문자열 또는 공백 문자열입니다.");
            return null;
        }

        try
        {
            TextAsset jsonTextAsset = await GameManager.Instance.ResourceManager.LoadAssetAsync<TextAsset>(key);

            if (jsonTextAsset == null)
            {
                throw new InvalidOperationException($"'{key}' 데이터 TextAsset 로드에 실패했습니다.");
            }

            List<T> items = JsonConvert.DeserializeObject<List<T>>(jsonTextAsset.text);

            if (items == null)
            {
                throw new InvalidOperationException($"'{key}' JSON 역직렬화 결과가 null입니다.");
            }

            Dictionary<string, T> dataTable = new();

            foreach (T item in items)
            {
                if (string.IsNullOrWhiteSpace(item.DataId))
                {
                    throw new FormatException($"'{key}' DataId가 없는 데이터가 존재합니다.");
                }

                if (!dataTable.TryAdd(item.DataId, item))
                {
                    throw new FormatException($"'{key}' 중복된 DataId '{item.DataId}'가 존재합니다.");
                }
            }

            return dataTable;
        }
        catch (JsonException exception)
        {
            Debug.LogError($"[DataManager:LoadDataTableAsync] JSON 파싱에 실패했습니다.\n{exception}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"[DataManager:LoadDataTableAsync] 데이터 테이블 로드 중 오류가 발생했습니다.\n{exception}");
        }

        return null;
    }
}