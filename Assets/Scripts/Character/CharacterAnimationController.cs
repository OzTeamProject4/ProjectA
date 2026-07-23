using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");

    private BattleCharacter _battleCharacter;
    private Animator _animator;
    private CharacterSkillSystem _skillSystem;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _battleCharacter = GetComponentInParent<BattleCharacter>();
        
        if ( _battleCharacter == null )
        {
            Debug.LogError("BattleCharacter 컴포넌트 null");
            return;
        }

        _skillSystem = GetComponentInParent<CharacterSkillSystem>();

        if (_skillSystem == null)
        {
            Debug.LogError("CharacterSkillSystem 컴포넌트 null");
        }
    }

    private void OnEnable()
    {
        if (_battleCharacter == null)
        {
            Debug.LogError("BattleCharacter 컴포넌트 null");
            return;
        }

        if (_skillSystem == null)
        {
            Debug.LogError("skillsystem이 null");
            return;
        }

        _battleCharacter.OnMoveSpeedChanged += HandleMoveSpeedChanged;
        _battleCharacter.OnGroundedChanged += HandleGroundedChanged;
        _skillSystem.OnSkillUsed += HandleSkillUsed;
    }

    private void OnDisable()
    {
        if (_battleCharacter != null)
        {
            _battleCharacter.OnMoveSpeedChanged -= HandleMoveSpeedChanged;
            _battleCharacter.OnGroundedChanged -= HandleGroundedChanged;
        }

        if (_skillSystem != null)
        {
            _skillSystem.OnSkillUsed -= HandleSkillUsed;
        }
    }
    private void HandleMoveSpeedChanged(float speed)
    {
        _animator.SetFloat(MoveSpeedHash, speed);
    }

    private void HandleGroundedChanged(bool isGrounded)
    {
        _animator.SetBool(IsGroundedHash, isGrounded);
    }

    private void HandleSkillUsed(CharacterSkillCategory category)
    {
        switch (category)
        {
            case CharacterSkillCategory.Basic:
                _animator.SetTrigger(AttackTriggerHash);
                break;
            case CharacterSkillCategory.Normal:
                _animator.SetTrigger(AttackTriggerHash);
                break;
            case CharacterSkillCategory.Ultimate:
                _animator.SetTrigger(AttackTriggerHash);
                break;
        }
    }
}
