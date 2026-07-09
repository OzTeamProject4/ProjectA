using Newtonsoft.Json;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

// TODO 희준 추후 매니저 완성시 교체해야 할 임시 로더
public class TempCharacterDataLoader : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private GameObject _testPlayerPrefab;

    private List<CharacterData> _characters;
    private List<BattleCharacter> _battleCharacterList;
    private void Awake()
    {

        if (_cinemachineCamera == null)
        {
            Debug.LogError("카메라 참조가 null. 인스펙터 확인");
            return;
        }

        TextAsset jsonFile = Resources.Load<TextAsset>("CharacterData");
        if (jsonFile == null)
        {
            Debug.LogError("jsonFile ResourceLoad가 null. Resources폴더 경로 확인");

            return;
        }

        string json = jsonFile.text;

        _characters = JsonConvert.DeserializeObject<List<CharacterData>>(json);

        if (_characters == null || _characters.Count == 0)
        {
            Debug.LogError("엑셀 캐릭터 데이터 확인");
            return;
        }

        Debug.Log($"캐릭터 데이터{_characters.Count}개 로드");

        foreach (CharacterData character in _characters)
        {
            Debug.Log($"{character.DataId}, {character.Name},{string.Join(",",character.SkillList)}, {character.ElementType}, {character.Role}");
        }

        if (_testPlayerPrefab == null)
        {
            Debug.LogError("testPlayerPrefab이 null");
            return;
        }

        _battleCharacterList = new List<BattleCharacter>();

        for (int i =0; i < 3; i++)
        {
            GameObject obj = Instantiate(_testPlayerPrefab, new Vector3(i * 3, 2, 0), Quaternion.identity);
            BattleCharacter battleCharacter = obj.GetComponent<BattleCharacter>();
            battleCharacter.Initialize(_characters[i]);
            _battleCharacterList.Add(battleCharacter);
        }

        GameObject partyObj = new GameObject("PartyController");
        PartyController partyController = partyObj.AddComponent<PartyController>();
        partyController.Initialize(_battleCharacterList, _cinemachineCamera);
    }
}
