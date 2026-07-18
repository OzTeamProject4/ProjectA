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

    private void Awake()
    {
        _agent = this.GetRequiredComponent<NavMeshAgent>();

        if (null == _camera)
        {
            _camera = Camera.main;
        }
    }

    private void Update()
    {
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

    public void StopMove()
    {
        if (null == _agent)
        {
            return;
        }

        _agent.ResetPath();
        _agent.isStopped = true;
    }

    public void ResumeMove()
    {
        if (null == _agent)
        {
            return;
        }

        _agent.isStopped = false;
    }

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
        if (!NavMesh.SamplePosition(worldPosition, out NavMeshHit navHit, _sampleMaxDistance, NavMesh.AllAreas))
        {
            Debug.Log($"[StagePlayerParty] NavMesh 위 유효한 지점을 찾지 못했습니다. pos={worldPosition}");
            return;
        }

        _agent.SetDestination(navHit.position);
    }
}