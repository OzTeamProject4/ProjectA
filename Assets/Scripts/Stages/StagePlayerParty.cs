using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NavMeshAgent))]
public class StagePlayerParty : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _clickableMask;
    [SerializeField] private float _sampleMaxDistance = 2f;

    private NavMeshAgent _agent;

    private StagePlayerPartyViewModel _viewModel;
    private bool _isSubscribed;

    private void Awake()
    {
        _agent = this.GetRequiredComponent<NavMeshAgent>();

        if (null == _camera)
        {
            _camera = Camera.main;
        }
    }

    private void OnEnable()
    {
        ApplyCanMove(null == _viewModel || _viewModel.CanMove);
    }

    private void OnDestroy()
    {
        Unsubscribe();

        _viewModel = null;
    }

    private void Update()
    {
        if (null != _viewModel && !_viewModel.CanMove)
        {
            return;
        }

        Mouse mouse = Mouse.current;

        if (null == mouse)
        {
            return;
        }

        if (!mouse.leftButton.wasPressedThisFrame)
        {
            return;
        }

        if (null != EventSystem.current && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        HandleClick(mouse.position.ReadValue());
    }

    // ===== ViewModel 바인딩 =====

    public void Bind(StagePlayerPartyViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("[StagePlayerParty] Bind: viewModel 이 null 입니다.");
            return;
        }

        Unsubscribe();

        _viewModel = viewModel;

        Subscribe();

        _viewModel.Refresh();
    }

    private void Subscribe()
    {
        if (_isSubscribed || null == _viewModel)
        {
            return;
        }

        _viewModel.OnCanMoveChanged += HandleCanMoveChanged;
        _viewModel.OnActiveChanged += HandleActiveChanged;

        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
        {
            return;
        }

        if (null != _viewModel)
        {
            _viewModel.OnCanMoveChanged -= HandleCanMoveChanged;
            _viewModel.OnActiveChanged -= HandleActiveChanged;
        }

        _isSubscribed = false;
    }

    private void HandleCanMoveChanged(bool canMove)
    {
        ApplyCanMove(canMove);
    }

    private void HandleActiveChanged(bool isActive)
    {
        if (!isActive)
        {
            ClearPath();
        }

        gameObject.SetActive(isActive);
    }

    // ===== 이동 제어 =====

    private void ApplyCanMove(bool canMove)
    {
        if (!IsAgentUsable())
        {
            return;
        }

        if (canMove)
        {
            _agent.isStopped = false;
            return;
        }

        _agent.ResetPath();
        _agent.isStopped = true;
    }

    public void ClearPath()
    {
        if (!IsAgentUsable())
        {
            return;
        }

        _agent.ResetPath();
    }

    public void WarpTo(Vector3 position)
    {
        if (!IsAgentUsable())
        {
            Debug.LogWarning("[StagePlayerParty] WarpTo: 에이전트를 사용할 수 없는 상태입니다.");
            return;
        }

        if (!NavMesh.SamplePosition(position, out NavMeshHit navHit, _sampleMaxDistance, NavMesh.AllAreas))
        {
            Debug.LogWarning($"[StagePlayerParty] WarpTo: NavMesh 위 유효한 지점을 찾지 못했습니다. pos={position}");
            return;
        }

        _agent.Warp(navHit.position);
    }

    private bool IsAgentUsable()
    {
        if (null == _agent)
        {
            return false;
        }

        return _agent.isActiveAndEnabled && _agent.isOnNavMesh;
    }

    // ===== 클릭 이동 =====

    private void HandleClick(Vector2 screenPosition)
    {
        if (null == _camera)
        {
            Debug.LogWarning("[StagePlayerParty] _camera 가 할당되지 않았습니다.");
            return;
        }

        Ray ray = _camera.ScreenPointToRay(screenPosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _clickableMask))
        {
            return;
        }

        Vector3 targetPosition = ResolveTargetPosition(hit);

        MoveTo(targetPosition);
    }

    private Vector3 ResolveTargetPosition(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent(out StageMonsterParty party))
        {
            return party.transform.position;
        }

        return hit.point;
    }

    private void MoveTo(Vector3 worldPosition)
    {
        if (!IsAgentUsable())
        {
            return;
        }

        if (!NavMesh.SamplePosition(worldPosition, out NavMeshHit navHit, _sampleMaxDistance, NavMesh.AllAreas))
        {
            Debug.Log($"[StagePlayerParty] NavMesh 위 유효한 지점을 찾지 못했습니다. pos={worldPosition}");
            return;
        }

        _agent.SetDestination(navHit.position);
    }
}
