using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

// TODO 희준 : 임시 파티 스포너, 추후 전투 씬 매니저/파티 편성 연동시 정리
public class TempPartySpawner
{
    public async UniTask<List<BattleCharacter>> SpawnPartyById(IReadOnlyList<string> partyDataId, Vector3 spawnOrigin)
    {
        List<BattleCharacter> characters = new List<BattleCharacter>();
        // IGameDataProvider provider = new GameDataProvider();

        int index = 0;
        foreach (string dataId in partyDataId)
        {
            GameManager.Instance.DataManager.TryGetData<CharacterData>(dataId, out var characterData);
            if (dataId == null)
            {
                Debug.LogError($"{dataId} 등록되지 않은 ID입니다");
                continue;
            }

            GameObject prefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(characterData.PrefabPath);
            if (prefab == null)
            {
                Debug.LogError("캐릭터 프리팹 로드 실패");
                return characters;
            }

            if (!GameManager.Instance.DataManager.TryGetData<CharacterData>(dataId, out CharacterData data))
            {
                Debug.LogError($"DataId {dataId} 캐릭터를 찾을 수 없습니다.");
                continue;
            }

            //CharacterGradeData grade = provider.GetGrade(data.Star);
            //StatData stats = StatCalculator.Calculate(data, grade, data.Level, default);

            Vector3 spawnPosition = spawnOrigin + new Vector3(index * 3, 2, 0);
            GameObject obj = Object.Instantiate(prefab, spawnPosition, Quaternion.identity); BattleCharacter battleCharacter = obj.GetComponent<BattleCharacter>();
            await battleCharacter.InitializeAsync(data);
            characters.Add(battleCharacter);
            index++;
        }

        return characters;
    }
}
