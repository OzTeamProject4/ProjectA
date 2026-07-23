using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DataManager : BaseManager<DataManager>
{
    private readonly Dictionary<Type, object> _dataTables = new Dictionary<Type, object>();

    private const int LoadingTransitionDelay = 1000;

    public override async UniTask InitializeAsync()
    {
        _dataTables.Clear();
        await PreloadDataAsync();
    }

    public async UniTask PreloadDataAsync(IProgress<LoadingProgress> progress = null)
    {
        await UniTask.Delay(1);
    }

    public async UniTask LoadRuntimeDataAsync(IProgress<LoadingProgress> progress = null)
    {
        List<LoadingTask> loadingTasks = CreateLoadingTasks();

        ReportLoadingProgress(progress, 0f, LoadingStep.Initialize);

        await UniTask.Delay(LoadingTransitionDelay);

        float progressStep = 1f / loadingTasks.Count;

        for (int index = 0; index < loadingTasks.Count; index++)
        {
            LoadingTask loadingTask = loadingTasks[index];

            await loadingTask.Execute(loadingTask.Key, destroyCancellationToken);

            float progressValue = progressStep * (index + 1);

            ReportLoadingProgress(progress, progressValue, loadingTask.Step);
        }

        ReportLoadingProgress(progress, 1f, LoadingStep.Complete);

        await UniTask.Delay(LoadingTransitionDelay);
    }

    private List<LoadingTask> CreateLoadingTasks()
    {
        List<LoadingTask> jobs = new List<LoadingTask>
        {
            new LoadingTask(LoadingStep.LoadCharacterData, AddressableKey.Data.CharacterData, LoadDataAsync<CharacterData>),
            new LoadingTask(LoadingStep.LoadCharacterGradeData, AddressableKey.Data.CharacterGrade, LoadDataAsync<CharacterGradeData>),
            new LoadingTask(LoadingStep.LoadLevelExpData, AddressableKey.Data.LevelExp, LoadDataAsync<LevelExpData>),
            new LoadingTask(LoadingStep.LoadItemData, AddressableKey.Data.Item, LoadDataAsync<ItemData>),
            new LoadingTask(LoadingStep.LoadCurrencyData, AddressableKey.Data.Currency, LoadDataAsync<CurrencyData>),
            new LoadingTask(LoadingStep.LoadEquipmentData, AddressableKey.Data.Equipment, LoadDataAsync<EquipmentData>),
            new LoadingTask(LoadingStep.LoadSignatureData, AddressableKey.Data.Signature, LoadDataAsync<SignatureData>),
            new LoadingTask(LoadingStep.LoadSkillData, AddressableKey.Data.CharacterSkill, LoadDataAsync<CharacterSkillData>),
                        new LoadingTask(LoadingStep.LoadStageData, AddressableKey.Data.Stage, LoadDataAsync<StageData>),
            new LoadingTask(LoadingStep.LoadStageWaveData, AddressableKey.Data.StageWave, LoadDataAsync<StageWaveData>),
            new LoadingTask(LoadingStep.LoadEnemyData, AddressableKey.Data.Enemy, LoadDataAsync<EnemyData>),
            new LoadingTask(LoadingStep.LoadEnemySkillData, AddressableKey.Data.EnemySkill, LoadDataAsync<EnemySkillData>)



        };

        return jobs;
    }

    private void ReportLoadingProgress(IProgress<LoadingProgress> progress, float value, LoadingStep step)
    {
        if (progress == null)
        {
            return;
        }

        progress.Report(new LoadingProgress(value, step));
    }

    public bool TryGetDataTable<T>(out Dictionary<string, T> dataTable) where T : BaseData
    {
        dataTable = null;

        if (!_dataTables.TryGetValue(typeof(T), out object loadedDataTable))
        {
            Debug.LogError($"[{nameof(DataManager)}:{nameof(TryGetDataTable)}] '{typeof(T).Name}' 데이터 테이블이 로드되어 있지 않습니다.");
            return false;
        }

        if (loadedDataTable is not Dictionary<string, T> typeCastDataTable)
        {
            Debug.LogError($"[{nameof(DataManager)}:{nameof(TryGetDataTable)}] '{typeof(T).Name}' 로드된 데이터 테이블과 타입이 일치하지 않습니다.");
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
            Debug.LogError($"[{nameof(DataManager)}:{nameof(TryGetData)}] 전달된 DataId가 null이거나 빈 문자열 또는 공백 문자열입니다.");
            return false;
        }

        if (!TryGetDataTable(out Dictionary<string, T> dataTable))
        {
            return false;
        }

        if (!dataTable.TryGetValue(dataId, out data))
        {
            Debug.LogError($"[{nameof(DataManager)}:{nameof(TryGetData)}] '{typeof(T).Name}' 데이터 테이블에서 DataId '{dataId}'를 찾을 수 없습니다.");
            return false;
        }

        return true;
    }

    private async UniTask LoadDataAsync<T>(string key, CancellationToken cancellationToken) where T : BaseData
    {
        Dictionary<string, T> loadedDataTable = await LoadDataTableAsync<T>(key, cancellationToken);

        if (loadedDataTable == null)
        {
            Debug.LogError($"[{nameof(DataManager)}:{nameof(LoadDataAsync)}] '{key}' 데이터 테이블을 로드하지 못했습니다.");
            return;
        }

        _dataTables[typeof(T)] = loadedDataTable;
    }

    private async UniTask<Dictionary<string, T>> LoadDataTableAsync<T>(string key, CancellationToken cancellationToken) where T : BaseData
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError($"[{nameof(DataManager)}:{nameof(LoadDataTableAsync)}] 전달된 key가 null이거나 빈 문자열 또는 공백 문자열입니다.");
            return null;
        }

        try
        {
            TextAsset jsonTextAsset = await GameManager.Instance.ResourceManager.LoadAssetAsync<TextAsset>(key, cancellationToken);

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
            Debug.LogError($"[{nameof(DataManager)}:{nameof(LoadDataTableAsync)}] JSON 파싱에 실패했습니다.\n{exception}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"[{nameof(DataManager)}:{nameof(LoadDataTableAsync)}] 데이터 테이블 로드 중 오류가 발생했습니다.\n{exception}");
        }

        return null;
    }
}