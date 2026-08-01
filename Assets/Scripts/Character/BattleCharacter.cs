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
    private const float JumpVelocityThreshold = 3.0f;
    private const float AnimSpeedDamping = 3.0f;
    private const float RunSpeedMultiplier = 2.0f;
    private const int GroundCheckBufferSize = 4;
    private const float FallMultiplier = 3.0f;
    private readonly Collider[] _groundCheckBuffer = new Collider[GroundCheckBufferSize];
    // TODO 희준 캐릭터 모델링시 수치 변화 필요
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _groundCheckRadius = 1.0f; 
    [SerializeField] private float _rotationSpeed = 4.0f;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private LayerMask _groundLayer;

    private StudentData _data;
    private Rigidbody _rigidbody;
    private float _curHp;
    private float _curAtk;
    private float _curDef;
    private float _curMoveSpeed;
    private float _curRunSpeed;
    private bool _wasGrounded;
    private float _currentAnimSpeed;
    private float _maxHp;
    private float _baseMoveSpeed;
    private bool _isDead;
    private CancellationTokenSource _buffCts;
    private NavMeshAgent _navMeshAgent;
    private Sprite _elementIcon;
    private Sprite _portraitSprite;

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
            return _curAtk;
        }
    }

    public int CurrentAttack
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

    public float MaxHp
    {
        get
        {
            return _maxHp;
        }
    }
    public string DataId
    {
        get
        {
            return _data.DataId;
        }
    }
    public string CharacterIconPath
    {
        get
        {
            return _data.CharacterIconPath;
        }
    }
    public Sprite ElementIcon
    {
        get
        {
            return _elementIcon;
        }
    }
    public Sprite PortraitSprite
    {
        get
        {
            return _portraitSprite;
        }
    }

    public bool IsDead
    {
        get
        {
            return _isDead;
        }
    }
    public event Action<float> OnMoveSpeedChanged;
    public event Action<bool> OnGroundedChanged;
    public event Action<float, float> OnHpChanged;
    public event Action<BattleCharacter> OnCharacterDied;

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

    private void FixedUpdate()
    {
        if (_rigidbody.linearVelocity.y < 0f)
        {
            _rigidbody.linearVelocity += Vector3.up * Physics.gravity.y * (FallMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    private void OnDestroy()
    {
        _buffCts?.Cancel();
        _buffCts?.Dispose();
    }
    public async UniTask InitializeAsync(StudentData data)
    {
        _data = data;

        float maxHp = data.MaxHp;
        float attack = data.Attack;
        float defense = data.Defence;
        float moveSpeed = data.MoveSpeed;

        if (NetworkManagerTemp.Instance != null)
        {
            if (NetworkManagerTemp.Instance.TryGetStudentStats(data.DataId, out StatData statData))
            {
                maxHp = statData.Hp;
                attack = statData.Attack;
                defense = statData.Defense;
                moveSpeed = statData.MoveSpeed;
            }
        }
        _maxHp = maxHp;
        SetHp(maxHp);

        _curAtk = attack;
        _curDef = defense;
        _curMoveSpeed = moveSpeed;
        _curRunSpeed = _curMoveSpeed * RunSpeedMultiplier;
        _baseMoveSpeed = moveSpeed;

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

        int hitCount = Physics.OverlapSphereNonAlloc(_groundCheckPoint.position, _groundCheckRadius, _groundCheckBuffer, _groundLayer, QueryTriggerInteraction.Ignore);
        return hitCount > 0;
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
    public void Teleport(Vector3 position)
    {
        _rigidbody.linearVelocity = Vector3.zero;
        transform.position = position;
        _rigidbody.position = position;

        if (_navMeshAgent != null && _navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.Warp(position);
        }
    }
    public void TakeDamage(int damage, GameObject attacker)
    {
        SetHp(_curHp - damage);
    }

    public void Heal(int amount)
    {
        SetHp(_curHp + amount);
    }

    public void ApplyMoveSpeedBuff(float moveSpeedBuff, float duration)
    {
        _buffCts?.Cancel();
        _buffCts?.Dispose();
        _buffCts = new CancellationTokenSource();
        ApplyMoveSpeedBuffAsync(moveSpeedBuff, duration, _buffCts.Token).Forget();
    }

    private void SetHp(float hp)
    {
        if (_isDead)
        {
            return;
        }

        _curHp = Mathf.Clamp(hp, 0f, _maxHp);
        OnHpChanged?.Invoke(_curHp, _maxHp);

        if (_curHp <= 0f)
        {
            Die();
        }

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
            Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);
            return;
        }

        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);
    }

    // 테스트용 임시코드
    [ContextMenu("Test Damage 100")]
    private void TestDamage()
    {
        SetHp(_curHp - 100);
    }
    public void SetElementIcon(Sprite icon)
    {
        _elementIcon = icon;
    }
    public void SetPortraitSprite(Sprite sprite)
    {
        _portraitSprite = sprite;
    }

    private void Die()
    {
        _isDead = true;
        _rigidbody.linearVelocity = Vector3.zero;
        OnCharacterDied?.Invoke(this);
    }
}
