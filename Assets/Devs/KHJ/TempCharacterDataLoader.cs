using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;


public class TempCharacterDataLoader : MonoBehaviour
{
    private List<CharacterData> _characters;
    private void Awake()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("CharacterData");
        string json = jsonFile.text;

        _characters = JsonConvert.DeserializeObject<List<CharacterData>>(json);

        Debug.Log($"캐릭터 데이터{_characters.Count}개 로드");

        foreach (CharacterData character in _characters)
        {
            Debug.Log($"{character.Id}, {character.Name}, {character.Level}, {character.BaseHp}, {character.BaseDef}, {character.BaseMoveSpeed}, {character.SkillGauge}, {string.Join(",",character.SkillList)}, {character.ElementType}, {character.Role}");
        }

        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        obj.name = "TestBattleCharacter";
        BattleCharacter battleCharacter = obj.AddComponent<BattleCharacter>();
        CharacterData testCharacter = _characters[0];
        battleCharacter.Initialize(testCharacter);

        Debug.Log($"현재 캐릭터정보 : {battleCharacter.CharacterName}, {battleCharacter.CurHp}");

        GameObject controllerObj = new GameObject("PlayerController");
        PlayerController playerController = controllerObj.AddComponent<PlayerController>();
        playerController.SetControlTarget(battleCharacter);
    }
}
