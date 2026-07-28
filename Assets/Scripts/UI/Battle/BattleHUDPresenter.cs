using System.Collections.Generic;
using UnityEngine;

public class BattleHUDPresenter
{
    private BattleHUDView _hudView;
    private PartyController _partyController;
    private BattleCharacter _currentCharacter;
    private CharacterSkillSystem _currentSkillSystem;
    private BattleTimer _battleTimer;

    public void Initialize(BattleHUDView hudView, PartyController partyController, BattleTimer battleTimer)
    {
        _hudView = hudView;
        _battleTimer = battleTimer;
        _partyController = partyController;
        _partyController.OnCharacterChanged += HandleCharacterChanged;
    }

    private void HandleCharacterChanged(BattleCharacter character)
    {
        if (_currentCharacter != null && _currentSkillSystem != null)
        {
            _currentCharacter.OnHpChanged -= HandleHpChanged;
            _currentSkillSystem.OnGaugeChanged -= HandleGaugeChanged;
        }

        _currentCharacter = character;
        if (character != null)
        {
            _currentSkillSystem = character.GetComponent<CharacterSkillSystem>();
        }

        else
        {
            _currentSkillSystem = null;
        }
        
        if (_currentCharacter != null && _currentSkillSystem != null)
        {
            _currentCharacter.OnHpChanged += HandleHpChanged;
            _currentSkillSystem.OnGaugeChanged += HandleGaugeChanged;
            HandleHpChanged(_currentCharacter.CurHp, _currentCharacter.MaxHp);
            HandleGaugeChanged(_currentSkillSystem.CurUltGauge, _currentSkillSystem.MaxUltGauge);
            _hudView.SetBasicSkillIcon(_currentSkillSystem.BasicSkillIcon);
            _hudView.SetNormalSkillIcon(_currentSkillSystem.NormalSkillIcon);
            _hudView.SetUltimateSkillIcon(_currentSkillSystem.UltimateSkillIcon);
            _hudView.SetCharacterIcon(_currentCharacter.PortraitSprite);

            if (NetworkManagerTemp.Instance != null)
            {
                StudentModel model = NetworkManagerTemp.Instance.StudentListModel.GetCharacter(_currentCharacter.DataId);
                if (model != null)
                {
                    _hudView.SetLevel(model.Level);
                }
            }
        }

        IReadOnlyList<int> waitingIndices = _partyController.WaitingMemberIndices;
        IReadOnlyList<BattleCharacter> party = _partyController.PartyCharacters;
        _hudView.SetPartyMemberCount(waitingIndices.Count);
        for (int i = 0; i < waitingIndices.Count; i++)
        {
            int partyIndex = waitingIndices[i];
            BattleCharacter member = party[partyIndex];
            if (member != null)
            {
                _hudView.SetPartyMemberPortrait(i, member.PortraitSprite);
                _hudView.SetPartyMemberNumber(i, partyIndex + 1);
                _hudView.SetPartyMemberElement(i, member.ElementIcon);
            }
        }
    }

    private void HandleHpChanged(float current, float max)
    {
        if (_hudView != null)
        {
            _hudView.SetPlayerHp(current, max);
        }
    }

    public void Cleanup()
    {
        if (_partyController != null )
        {
            _partyController.OnCharacterChanged -= HandleCharacterChanged;
        }

        if (_currentCharacter != null)
        {
            _currentCharacter.OnHpChanged -= HandleHpChanged;
        }

        if (_currentSkillSystem != null)
        {
            _currentSkillSystem.OnGaugeChanged -= HandleGaugeChanged;
        }
    }

    private void HandleGaugeChanged(int current, int max)
    {
        if (_hudView == null)
        {
            return;
        }

        float ratio = 0f;
        if (max >0)
        {
            ratio = (float)current / max;
        }

        _hudView.SetUltimateGauge(current, max);
    }

    public void Tick()
    {
        if (_battleTimer != null && _hudView != null)
        {
            _hudView.SetTimer(_battleTimer.RemainTime);
        }

        if (_hudView != null && _partyController != null)
        {
            IReadOnlyList<int> waitingIndices = _partyController.WaitingMemberIndices;
            IReadOnlyList<BattleCharacter> party = _partyController.PartyCharacters;
            for (int i = 0; i < waitingIndices.Count; i++)
            {
                int partyIndex = waitingIndices[i];
                BattleCharacter member = party[partyIndex];
                if (member != null)
                {
                    _hudView.SetPartyMemberHp(i, member.CurHp, member.MaxHp);

                    CharacterSkillSystem skillSystem = member.GetComponent<CharacterSkillSystem>();
                    if (skillSystem != null)
                    {
                        float ratio = 0f;
                        if (skillSystem.MaxUltGauge > 0)
                        {
                            ratio = (float)skillSystem.CurUltGauge / skillSystem.MaxUltGauge;
                        }
                        _hudView.SetPartyMemberGauge(i, ratio);
                    }
                }
            }
            
            _hudView.SetSwitchCooldown(_partyController.SwitchCooldownProgress);
        }

        if (_hudView == null || _currentSkillSystem == null)
        {
            return;
        }

        _hudView.SetBasicSkillCooldown(_currentSkillSystem.BasicSkillCooldownProgress);
        _hudView.SetNormalSkillCooldown(_currentSkillSystem.NormalSkillCooldownProgress);
    }
}
