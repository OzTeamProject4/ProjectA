using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PartyController
{
    private const float EffectLifeTime = 3.0f; // TODO 상수 모아야함

    private List<BattleCharacter> _partyCharacters;
    private int _currentCharacterIndex;
    private CinemachineCamera _cinemachineCamera;
    private float _switchCoolTime = 3.0f;
    private float _lastSwitchTime;
    private List<CharacterAIController> _aiControllerList;
    private List<PlayerController> _playerControllerList;
    public IReadOnlyList<BattleCharacter> PartyCharacters
    {
        get
        {
            return _partyCharacters;
        }
    }
    public float SwitchCooldownProgress
    {
        get
        {
            if (_switchCoolTime <= 0)
            {
                return 1.0f;
            }

            else
            {
                return Mathf.Clamp01((Time.time - _lastSwitchTime) / _switchCoolTime);
            }
        }
    }
    public IReadOnlyList<int> WaitingMemberIndices
    {
        get
        {
            List<int> waiting = new List<int>();
            if (_partyCharacters == null)
            {
                return waiting;
            }
            for (int i = 0; i < _partyCharacters.Count; i++)
            {
                if (i == _currentCharacterIndex)
                {
                    continue;
                }
                if (_partyCharacters[i] == null)
                {
                    continue;
                }
                waiting.Add(i);
            }
            return waiting;
        }
    }

    public event Action<BattleCharacter> OnCharacterChanged;
    public event Action OnPartyWiped;

    public void Initialize(List<BattleCharacter> characters, CinemachineCamera cinemachinCamera)
    {
        if (characters == null || characters.Count == 0)
        {
            Debug.LogError("[PartyController] characters 가 비어 있습니다.");
            return;
        }

        if (cinemachinCamera == null)
        {
            Debug.LogError("[PartyController] cinemachineCamera 가 null 입니다.");
            return;
        }

        _partyCharacters = characters;
        _cinemachineCamera = cinemachinCamera;
        _lastSwitchTime = -_switchCoolTime;

        SetupControllers();

        if (_partyCharacters.Count == 0)
        {
            Debug.LogError("[PartyController] 사용 가능한 캐릭터가 없습니다. 캐릭터 프리팹 구성을 확인하세요.");
            return;
        }

        SwitchCharacter(0);
    }

    private void SetupControllers()
    {
        _playerControllerList = new List<PlayerController>();
        _aiControllerList = new List<CharacterAIController>();

        List<BattleCharacter> validCharacters = new List<BattleCharacter>();

        for (int i = 0; i < _partyCharacters.Count; i++)
        {
            BattleCharacter character = _partyCharacters[i];

            if (character == null)
            {
                Debug.LogError("[PartyController] 파티 목록에 null 캐릭터가 있습니다.");
                continue;
            }

            PlayerController player = character.GetComponent<PlayerController>();
            CharacterAIController ai = character.GetComponent<CharacterAIController>();
            CharacterSkillSystem skillSystem = character.GetComponent<CharacterSkillSystem>();

            if (player == null || ai == null)
            {
                Debug.LogError($"{character.name}에 필요한 컨트롤러가 없음. 프리팹 확인");
                continue;
            }

            if (skillSystem != null)
            {
                skillSystem.OnHealBuffRequested += HandleHealBuff;
            }

            character.OnCharacterDied += HandleCharacterDied;
            player.enabled = false;
            ai.DisableAI();
            // ai.Initialize(_partyCharacters[0], character);

            _playerControllerList.Add(player);
            _aiControllerList.Add(ai);
            validCharacters.Add(character);
        }

        _partyCharacters = validCharacters;
    }

    public void SwitchCharacter(int index)
    {
        if (_partyCharacters == null || index < 0 || index >= _partyCharacters.Count)
        {
            Debug.LogError($"[PartyController] SwitchCharacter: 잘못된 인덱스입니다. index={index}");
            return;
        }

        if (_cinemachineCamera == null)
        {
            Debug.LogError("[PartyController] SwitchCharacter: _cinemachineCamera 가 null 입니다.");
            return;
        }

        _currentCharacterIndex = index;
        BattleCharacter target = _partyCharacters[index];

        for (int i = 0; i < _playerControllerList.Count; i++)
        {
            bool isSelected = (i == index);

            BattleCharacter member = _partyCharacters[i];
            bool isDead = (member != null && member.IsDead);

            _playerControllerList[i].enabled = isSelected;

            if (isSelected || isDead)
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
        SetControlCharacter(target);
    }
   
    public void TrySwitchToCharacter(int index)
    {
        if (_partyCharacters == null)
        {
            return;
        }

        if (index < 0 || index >= _partyCharacters.Count)
        {
            return;
        }

        if (index == _currentCharacterIndex)
        {
            return;
        }

        BattleCharacter target = _partyCharacters[index];

        if (target == null || target.IsDead)
        {
            return;
        }

        if (Time.time - _lastSwitchTime < _switchCoolTime)
        {
            return;
        }

        SwitchCharacter(index);
        _lastSwitchTime = Time.time;
    }

    public void UseCurrentCharacterUlt()
    {
        if (!IsCurrentCharacterValid())
        {
            return;
        }

        BattleCharacter current = _partyCharacters[_currentCharacterIndex];

        CharacterSkillSystem skillSystem = current.GetComponent<CharacterSkillSystem>();
        if (skillSystem != null)
        {
            skillSystem.UseUltSkill();
        }
    }

    public void UseCurrentCharacterBasicSkill()
    {
        if (!IsCurrentCharacterValid())
        {
            return;
        }

        BattleCharacter current = _partyCharacters[_currentCharacterIndex];
        CharacterSkillSystem skillSystem = current.GetComponent<CharacterSkillSystem>();
        if (skillSystem != null)
        {
            skillSystem.UseBasicSkill();
        }
    }

    public void UseCurrentCharacterNormalSkill()
    {
        if (!IsCurrentCharacterValid())
        {
            return;
        }

        BattleCharacter current = _partyCharacters[_currentCharacterIndex];
        CharacterSkillSystem skillSystem = current.GetComponent<CharacterSkillSystem>();
        if (skillSystem != null)
        {
            skillSystem.UseNormalSkill();
        }
    }

    private bool IsCurrentCharacterValid()
    {
        if (_partyCharacters == null)
        {
            return false;
        }

        if (_currentCharacterIndex < 0 || _currentCharacterIndex >= _partyCharacters.Count)
        {
            return false;
        }

        return _partyCharacters[_currentCharacterIndex] != null;
    }

    public void Cleanup()
    {
        if (_partyCharacters == null)
        {
            return;
        }

        for (int i = 0; i < _partyCharacters.Count; i++)
        {
            BattleCharacter character = _partyCharacters[i];
            if (character == null)
            {
                continue;
            }

            character.OnCharacterDied -= HandleCharacterDied;

            CharacterSkillSystem skillSystem = character.GetComponent<CharacterSkillSystem>();
            if (skillSystem != null)
            {
                skillSystem.OnHealBuffRequested -= HandleHealBuff;
            }

            UnityEngine.Object.Destroy(character.gameObject);
        }

        _partyCharacters.Clear();
        _playerControllerList = null;
        _aiControllerList = null;
    }

    private void HandleHealBuff(int healAmount, float buffDuration, float moveSpeedBuff, GameObject effectPrefab)
    {
        foreach (BattleCharacter character in _partyCharacters)
        {
            if (character == null)
            {
                continue;
            }
            
            character.Heal(healAmount);
            character.ApplyMoveSpeedBuff(moveSpeedBuff, buffDuration);
            if (effectPrefab != null)
            {
                GameObject effect = UnityEngine.Object.Instantiate(effectPrefab, character.transform);
                effect.transform.localPosition = Vector3.zero;
                UnityEngine.Object.Destroy(effect, EffectLifeTime);
            }
        }
    }

    private void SetControlCharacter(BattleCharacter character)
    {
        OnCharacterChanged?.Invoke(character);
    }

    private void HandleCharacterDied(BattleCharacter character)
    {
        if (character == null)
        {
            Debug.LogError("[PartyController] HandleCharacterDied: character 가 null 입니다.");
            return;
        }

        int diedIndex = _partyCharacters.IndexOf(character);

        if (diedIndex < 0)
        {
            Debug.LogError($"[PartyController] 파티에 없는 캐릭터의 사망 신호입니다. name={character.name}");
            return;
        }

        _playerControllerList[diedIndex].enabled = false;
        _aiControllerList[diedIndex].DisableAI();

        int nextIndex = FindNextAliveIndex();

        if (nextIndex < 0)
        {
            Debug.Log("[PartyController] 파티 전멸");
            OnPartyWiped?.Invoke();
            return;
        }

        if (diedIndex != _currentCharacterIndex)
        {
            return;
        }

        SwitchCharacter(nextIndex);
    }

    private int FindNextAliveIndex()
    {
        if (_partyCharacters == null)
        {
            return -1;
        }

        for (int i = 0; i < _partyCharacters.Count; i++)
        {
            BattleCharacter character = _partyCharacters[i];

            if (character == null)
            {
                continue;
            }

            if (character.IsDead)
            {
                continue;
            }

            return i;
        }

        return -1;
    }
}
