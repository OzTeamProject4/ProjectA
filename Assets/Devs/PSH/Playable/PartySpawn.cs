using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class PartySpawn : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _spawnHeightOffset = 1f;
    [SerializeField] private Transform _spawnParent;


    //partyDataIds라는 캐릭터 ID 목록 수신 -> 비동기 작업으로 캐릭터들을 생성
    // -> 생성된 BattleCharacter 목록을 반환
    public async UniTask<List<BattleCharacter>> SpawnPartyById(List<string> partyDataIds)
    {
        // 이 리스트 안에 파티 캐릭터 ID 있음
        List<BattleCharacter> characters = new List<BattleCharacter>();

        // 파티에 들어간 캐릭터 ID가 없으면?
        if (partyDataIds == null || partyDataIds.Count == 0)
        {
            Debug.LogWarning("No party ids are assigned.");
            return characters;
        }

        // 스폰 포인트가 없으면?
        if (_spawnPoints == null || _spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points are assigned.");
            return characters;
        }

        // 파티에 속한 캐릭터들의 프리팹 비동기 로딩
        GameObject prefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>("Prefab_TestBattleCharacterBase");


        // 프리팹이 없으면?
        if (prefab == null)
        {
            Debug.LogError("Character prefab load failed.");
            return characters;
        }


        // 파티원 수 많큼 순회
        for (int i = 0; i < partyDataIds.Count; i++)
        {
            // 스폰 포인트 보다 파티원 수가 많으면
            if (i >= _spawnPoints.Length)
            {
                Debug.LogWarning($"Spawn point for party index {i} is missing.");
                continue;
            }

            // 파티원 id 저장
            string dataId = partyDataIds[i];

            // DataManager 에서 dataID에 해당하는 CharacterData를 탐색
            if (!GameManager.Instance.DataManager.TryGetData<CharacterData>(dataId, out CharacterData data))
            {
                // 없다면?
                Debug.LogError($"CharacterData not found. DataId: {dataId}");
                continue;
            }

            Transform spawnPoint = _spawnPoints[i];

            // i 번째 스폰 포인트가 없다면
            if (spawnPoint == null)
            {
                Debug.LogError($"Spawn point at index {i} is missing.");
                continue;
            }

            // 파티에 속한 캐릭터 오브젝트 생성
            GameObject obj = Instantiate(
                prefab,
                spawnPoint.position + Vector3.up * _spawnHeightOffset,
                spawnPoint.rotation,
                _spawnParent
            );


            BattleCharacter battleCharacter = obj.GetComponent<BattleCharacter>();

            // BattleCharacter 컴포넌트가 없다면?
            if (battleCharacter == null)
            {
                Debug.LogError($"{obj.name} does not have BattleCharacter.");
                continue;
            }

            // 찾아온 캐릭터 정보 데이터 적용
            battleCharacter.Initialize(data);
            characters.Add(battleCharacter);
        }

        return characters;
    }
}