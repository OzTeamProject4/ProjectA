using UnityEngine;
using UnityEngine.Rendering.UI;

public class BattleHUDPresenter
{
    private BattleHUDView _hudView;
    private PartyController _partyController;
    private BattleCharacter _currentCharacter;
    private CharacterSkillSystem _currentSkillSystem;

    public void Initialize(BattleHUDView hudView, PartyController partyController)
    {
        _hudView = hudView;
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

        _hudView.SetUltimateGauge(ratio);
    }

    public void TickSkill()
    {
        if (_hudView == null || _currentSkillSystem == null)
        {
            return;
        }

        _hudView.SetBasicSkillCooldown(_currentSkillSystem.BasicSkillCooldownProgress);
        _hudView.SetNormalSkillCooldown(_currentSkillSystem.NormalSkillCooldownProgress);

    }
}
