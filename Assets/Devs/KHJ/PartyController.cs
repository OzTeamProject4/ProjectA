using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PartyController
{
    private List<BattleCharacter> _partyCharacters;
    private int _currentCharacterIndex;
    private CinemachineCamera _cinemachineCamera;
    private float _switchCoolTime = 3.0f;
    private float _lastSwitchTime;
    private List<CharacterAIController> _aiControllerList;
    private List<PlayerController> _playerControllerList;

    

    public void Initialize(List<BattleCharacter> characters, CinemachineCamera cinemachinCamera)
    {
        _partyCharacters = characters;
        _cinemachineCamera = cinemachinCamera;

        SetupControllers();
        SwitchCharacter(0);
    }

    private void SetupControllers()
    {
        _playerControllerList = new List<PlayerController>();
        _aiControllerList = new List<CharacterAIController>();

        for (int i = 0; i < _partyCharacters.Count; i++)
        {
            BattleCharacter character = _partyCharacters[i];
            PlayerController player = character.GetComponent<PlayerController>();
            CharacterAIController ai = character.GetComponent<CharacterAIController>();

            if (player == null || ai == null)
            {
                Debug.LogError($"{character.name}에 필요한 컨트롤러가 없음. 프리팹 확인");
                return;
            }

            player.enabled = false;
            ai.DisableAI();
            // ai.Initialize(_partyCharacters[0], character);

            _playerControllerList.Add(player);
            _aiControllerList.Add(ai);
        }


    }

    public void SwitchCharacter(int index)
    {

        _currentCharacterIndex = index;
        BattleCharacter target = _partyCharacters[index];

        for (int i = 0; i < _playerControllerList.Count; i++)
        {
            bool isSelected = (i == index);

            _playerControllerList[i].enabled = isSelected;

            if (isSelected)
            {
                _aiControllerList[i].DisableAI();
            }

            else
            {
                _aiControllerList[i].EnableAI();
            }

            _aiControllerList[i].SetAIFollowTarget(target);
        }

        _cinemachineCamera.Target.TrackingTarget = target.transform;
    }
   
    public void TrySwitchToNextCharacter()
    {
        if (Time.time - _lastSwitchTime < _switchCoolTime)
        {
            // TODO 희준 : 추후 UI에 표시 필요
            Debug.Log("아직 캐릭터 태그 기능 사용할수 없습니다");
            return;
        }

        int nextIndex = (_currentCharacterIndex + 1) % _partyCharacters.Count;
        SwitchCharacter(nextIndex);
        _lastSwitchTime = Time.time;
    }

    public void UseCurrentCharacterUlt()
    {
        BattleCharacter current = _partyCharacters[_currentCharacterIndex];
        Debug.Log($"궁 발동 시도 {current.CharacterName}");

        CharacterSkillSystem skillSystem = current.GetComponent<CharacterSkillSystem>();
        if (skillSystem != null)
        {
            skillSystem.UseUltSkill();
        }
    }
}
