using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

// TODO 희준 : 임시 파티 스포너, 추후 전투 씬 매니저/파티 편성 연동시 정리
public class TempPartySpawner
{
    public async UniTask<List<BattleCharacter>> SpawnPartyById(IReadOnlyList<string> partyDataId, Vector3 spawnOrigin)
    {
        List<BattleCharacter> characters = new List<BattleCharacter>();

        int index = 0;
        foreach (string dataId in partyDataId)
        {
            if (!GameManager.Instance.DataManager.TryGetData<StudentData>(dataId, out StudentData data))
            {
                Debug.LogError($"DataId {dataId} 캐릭터를 찾을 수 없습니다.");
                continue;
            }

            GameObject prefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(data.PrefabPath);
            if (prefab == null)
            {
                Debug.LogError("캐릭터 프리팹 로드 실패");
                return characters;
            }

            Vector3 spawnPosition = spawnOrigin + new Vector3(index * 3, 2, 0);
            GameObject obj = Object.Instantiate(prefab, spawnPosition, Quaternion.identity);
            BattleCharacter battleCharacter = obj.GetComponent<BattleCharacter>();

            if (battleCharacter == null)
            {
                Debug.LogError($"{obj.name} 에 BattleCharacter 가 없습니다.");
                continue;
            }

            await battleCharacter.InitializeAsync(data);
            characters.Add(battleCharacter);
            index++;
        }

        return characters;
    }
}
