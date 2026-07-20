using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");

    private Animator _animatior;

    private void Awake()
    {
        _animatior = GetComponent<Animator>();
    }

    public void SetMoveSpeed(float speed)
    {
        _animatior.SetFloat(MoveSpeedHash, speed);
    }
}
