using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

// TODO 희준 : 임시 파티 스포너, 추후 전투 씬 매니저/파티 편성 연동시 정리
public class TempPartySpawner : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private GameObject _testPlayerPrefab;

    
    private List<BattleCharacter> _battleCharacterList;

    private async void Start()
    {
        if (_cinemachineCamera == null)
        {
            Debug.LogError("카메라 참조가 null. 인스펙터 확인");
            return;
        }

        if(_testPlayerPrefab == null)
        {
            Debug.LogError("프리팹이 null");
            return;
        }

        await GameManager.Instance.DataManager.LoadDataAsync<CharacterData>("Data_TestCharacter");

        // TODO 희준 임시 파티ID 추후 파티 편성창에서 전달받는 방식으로 교체
        List<string> tempPartyIds = new List<string> { "Character_001", "Character_003", "Character_005" };
        CreatePartyById(tempPartyIds);

        GameObject partyObj = new GameObject("PartyController");
        PartyController partyController = partyObj.AddComponent<PartyController>();
        partyController.Initialize(_battleCharacterList, _cinemachineCamera);
    }

    private void CreatePartyById(List<string> partyDataIds)
    {
        _battleCharacterList = new List<BattleCharacter>();
        int index = 0;
        foreach (string dataId in partyDataIds)
        {
            if (!GameManager.Instance.DataManager.TryGetData<CharacterData>(dataId, out CharacterData character))
            {
                Debug.LogError($"DataId {dataId} 캐릭터를 찾을수 없습니다");
                continue;
            }

            GameObject obj = Instantiate(_testPlayerPrefab, new Vector3(index * 3, 2, 0), Quaternion.identity);
            BattleCharacter battleCharacter = obj.GetComponent<BattleCharacter>();
            battleCharacter.Initialize(character);
            _battleCharacterList.Add(battleCharacter);

            Debug.Log($"현재 생성된 캐릭터 정보 {battleCharacter.CharacterName}");
            index++;
        }
    }
}
