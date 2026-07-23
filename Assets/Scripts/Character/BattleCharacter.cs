using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]

public class BattleCharacter : MonoBehaviour, IDamageable
{
    private const float MoveThreshold = 0.1f;
    private const float WalkSpeedRatio = 0.5f;
    private const float RunSpeedRatio = 1.0f;
    private const float JumpVelocityThreshold = 1.0f;
    private const float AnimSpeedDamping = 3.0f;
    private const float RunSpeedMultiplier = 2.0f;
    // TODO 희준 캐릭터 모델링시 수치 변화 필요
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _groundCheckDistance = 0.1f; 
    [SerializeField] private float _rotationSpeed = 4.0f;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private LayerMask _groundLayer;

    private CharacterData _data;
    private Rigidbody _rigidbody;
    private float _curHp;
    private int _curSkillGauge;
    private float _curAtk;
    private float _curDef;
    private float _curMoveSpeed;
    private float _curRunSpeed;
    private bool _wasGrounded;
    private float _currentAnimSpeed;
    private float _maxHp;
    private float _baseMoveSpeed;
    private CancellationTokenSource _buffCts;
    private NavMeshAgent _navMeshAgent;

    public string CharacterName
    {
        get
        {
            return _data.Name;
        }
    }
    public float CurHp
    {
        get
        {
            return _curHp;
        }
    }
    
    public float CurAtk
    {
        get
        {
            return _curAtk; // 임시 자료형 통합
        }
    }

    public int CurrentAttack // 컴파일 통과용 프로퍼티 추가
    {
        get
        {
            return (int)_curAtk;
        }
    }

    public ElementType ElementType
    {
        get
        {
            return _data.Type;   
        }
    }
    public NavMeshAgent NavMeshAgent
    {
        get
        {
            return _navMeshAgent;
        }
    }

    public event Action<float> OnMoveSpeedChanged;
    public event Action<bool> OnGroundedChanged;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _navMeshAgent = GetComponent<NavMeshAgent>();

        if (_navMeshAgent != null)
        {
            _navMeshAgent.updatePosition = false;
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
        }
    }

    private void Update()
    {
        if (_groundCheckPoint == null)
        {
            return;
        }

        bool grounded = IsGrounded();
        if (grounded != _wasGrounded)
        {
            OnGroundedChanged?.Invoke(grounded);
            _wasGrounded = grounded;
        }
    }

    private void OnDestroy()
    {
        _buffCts?.Cancel();
        _buffCts?.Dispose();
    }

    
    public async UniTask InitializeAsync(CharacterData data, StatData stats)
    {
        _data = data;
        _curHp = stats.Hp;
        _curAtk = stats.Atk;
        _curDef = stats.Def;
        _curMoveSpeed = stats.MoveSpeed;
        _curRunSpeed = _curMoveSpeed * RunSpeedMultiplier;
        _curSkillGauge = 0;
        _maxHp = stats.Hp;
        _curMoveSpeed = stats.MoveSpeed;
        _curRunSpeed = _curMoveSpeed * RunSpeedMultiplier;
        _baseMoveSpeed = stats.MoveSpeed;

        CharacterSkillSystem skillSystem = GetComponent<CharacterSkillSystem>();
        if (skillSystem != null)
        {
            await skillSystem.InitializeAsync(data);
        }
    }

    public void Move(Vector3 moveDirection, bool isRunning, bool rotateToMoveDirection = true)
    {
        moveDirection.y = 0;
        float speed = isRunning ? _curRunSpeed : _curMoveSpeed;
        Vector3 velocity = moveDirection * speed;
        velocity.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = velocity;

        float inputMagnitude = moveDirection.magnitude;
        if (inputMagnitude > MoveThreshold)
        {
            if (_modelTransform != null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                _modelTransform.rotation = Quaternion.Slerp(_modelTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }

        float ratio = isRunning ? RunSpeedRatio : WalkSpeedRatio;
        float targetAnimSpeed = inputMagnitude * ratio;

        _currentAnimSpeed = Mathf.MoveTowards(_currentAnimSpeed, targetAnimSpeed, AnimSpeedDamping * Time.deltaTime);
        OnMoveSpeedChanged?.Invoke(_currentAnimSpeed);
    }

    public void Jump()
    {
        if (IsGrounded() == false)
        {
            return;
        }

        Vector3 velocity = _rigidbody.linearVelocity;
        velocity.y = _jumpForce;
        _rigidbody.linearVelocity = velocity;
    }

    private bool IsGrounded()
    {
        if (_groundCheckPoint == null)
        {
            Debug.LogError("_groundCehckPoint 확인");
            return false;
        }

        if (Mathf.Abs(_rigidbody.linearVelocity.y) > JumpVelocityThreshold)
        {
            return false;
        }

        bool result = Physics.CheckSphere(_groundCheckPoint.position, _groundCheckDistance, _groundLayer, QueryTriggerInteraction.Ignore);
        return result;
    }

    public void LookAt(Vector3 targetPosition)
    {
        if (_modelTransform == null)
        {
            return;
        }

        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0;

        if(direction.magnitude < MoveThreshold)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        _modelTransform.rotation = Quaternion.Slerp(_modelTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    public void LookAtInstant(Vector3 targetPosition)
    {
        if (_modelTransform == null)
        {
            return;
        }

        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0;
        
        if (direction.magnitude < MoveThreshold)
        {
            return;
        }

        _modelTransform.rotation = Quaternion.LookRotation(direction.normalized);
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        _curHp -= damage;
        if(_curHp < 0)
        {
            _curHp = 0;
        }
    }

    public void Heal(int amount)
    {
        _curHp += amount;
        if (_curHp > _maxHp)
        {
            _curHp = _maxHp;
        }
    }

    public void ApplyMoveSpeedBuff(float moveSpeedBuff, float duration)
    {
        _buffCts?.Cancel();
        _buffCts?.Dispose();
        _buffCts = new CancellationTokenSource();
        ApplyMoveSpeedBuffAsync(moveSpeedBuff, duration, _buffCts.Token).Forget();
    }

    private async UniTask ApplyMoveSpeedBuffAsync(float speedBuffPercent, float duration, CancellationToken token)
    {
        _curMoveSpeed = _baseMoveSpeed * (1 + speedBuffPercent / 100f);
        _curRunSpeed = _curMoveSpeed * RunSpeedMultiplier;

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
        }

        catch (OperationCanceledException)
        {
            return;
        }

        _curMoveSpeed = _baseMoveSpeed;
        _curRunSpeed = _curMoveSpeed * RunSpeedMultiplier;
    }

    private void OnDrawGizmos()
    {
        if (null == _groundCheckPoint)
        {
            return;
        }

        if (null == _rigidbody)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckDistance);
            return;
        }

        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckDistance);
    }
}
