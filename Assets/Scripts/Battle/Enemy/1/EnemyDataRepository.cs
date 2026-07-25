using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public sealed class EnemyDataRepository
{
    // Addressables JSON 주소
    private readonly string enemyDataJsonAddress;

    // Id 기준 적 정보 캐시
    // 데이터 ID - 적 스탯 정보 쌍
    private Dictionary<string, EnemyData> enemyDataById;


    public EnemyDataRepository(string enemyDataJsonAddress)
    {
        this.enemyDataJsonAddress = enemyDataJsonAddress;
    }

    // Id로 적 정보 찾기
    public async UniTask<EnemyData> GetByIdAsync(string Id, CancellationToken token)
    {

        // ID 없으면
        if (string.IsNullOrWhiteSpace(Id))
        {
            return null;
        }

        await LoadIfNeededAsync(token);

        // 데이터 ID에 맞는 적 정보 취득
        enemyDataById.TryGetValue(Id, out EnemyData enemyData);
        return enemyData;
    }


    private async UniTask LoadIfNeededAsync(CancellationToken token)
    {
        if (enemyDataById != null)
        {
            return;
        }

        // Addressables에서 JSON 파일 로드
        AsyncOperationHandle<TextAsset> handle =
            Addressables.LoadAssetAsync<TextAsset>(enemyDataJsonAddress);

        try
        {
            TextAsset jsonFile = await handle.ToUniTask(cancellationToken: token);

            // JSON 문자열을 적 정보 목록으로 변환
            // JSON -> C# 데이터
            List<EnemyData> loadedEnemyData = JsonConvert.DeserializeObject<List<EnemyData>>(jsonFile.text);


            if (loadedEnemyData == null)
            {
                throw new InvalidOperationException("Enemy data JSON is empty or invalid.");
            }


            // 빠른 검색을 위해 Dictionary에 저장
            enemyDataById = new Dictionary<string, EnemyData>();

            foreach (EnemyData enemyData in loadedEnemyData)
            {
                if (enemyData == null || string.IsNullOrWhiteSpace(enemyData.DataId))
                {
                    continue;
                }

                if (enemyDataById.ContainsKey(enemyData.DataId))
                {
                    throw new InvalidOperationException(
                        $"Duplicate enemy Id: {enemyData.DataId}");
                }

                enemyDataById.Add(enemyData.DataId, enemyData);
            }
        }
        finally
        {
            // JSON 에셋 로드 핸들 해제
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }
}
