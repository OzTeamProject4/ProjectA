using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    private BattleCharacter _battleCharacter;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _battleCharacter = GetComponentInParent<BattleCharacter>();
        if ( _battleCharacter == null )
        {
            Debug.LogError("BattleCharacter 컴포넌트 null");
            return;
        }
    }

    private void OnEnable()
    {
        if (_battleCharacter == null)
        {
            Debug.LogError("BattleCharacter 컴포넌트 null");
            return;
        }

        _battleCharacter.OnMoveSpeedChanged += HandleMoveSpeedChanged;
        _battleCharacter.OnGroundedChanged += HandleGroundedChanged;
    }

    private void OnDisable()
    {
        _battleCharacter.OnMoveSpeedChanged -= HandleMoveSpeedChanged;
        _battleCharacter.OnGroundedChanged -= HandleGroundedChanged;
    }
    private void HandleMoveSpeedChanged(float speed)
    {
        _animator.SetFloat(MoveSpeedHash, speed);
    }

    private void HandleGroundedChanged(bool isGrounded)
    {
        _animator.SetBool(IsGroundedHash, isGrounded);
    }
}
