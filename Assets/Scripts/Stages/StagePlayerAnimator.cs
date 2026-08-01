using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class StagePlayerAnimator : MonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

    [SerializeField] private Animator _animator;
    [SerializeField] private float _moveThreshold = 0.1f;

    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = this.GetRequiredComponent<NavMeshAgent>();

        if (null == _animator)
        {
            _animator = this.GetComponentInChildren<Animator>();
        }

        if (null == _animator)
        {
            Debug.LogError("[StagePlayerAnimator] Animator 를 찾지 못했습니다.");
        }
    }

    private void Update()
    {
        if (null == _animator)
        {
            return;
        }

        bool isMoving = IsAgentMoving();
        _animator.SetBool(IsMovingHash, isMoving);
    }

    private bool IsAgentMoving()
    {
        if (null == _agent)
        {
            return false;
        }

        float thresholdSqr = _moveThreshold * _moveThreshold;
        return _agent.velocity.sqrMagnitude > thresholdSqr;
    }
}