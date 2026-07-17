using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class BattleCharacter : MonoBehaviour
{
    private const float MoveThreshold = 0.1f;
    private const float WalkSpeedRatio = 0.5f;
    private const float RunSpeedRatio = 1.0f;
    private const float JumpVelocityThreshold = 1.0f;
    private const float AnimSpeedDamping = 3.0f;
    private const float RunSpeedMultiplier = 2.0f;
    // TODO 희준 캐릭터 모델링시 수치 변화 필요
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _groundCheckDistance = 0.05f; 
    [SerializeField] private float _rotationSpeed = 4.0f;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Transform _modelTransform;

    private CharacterData _data;
    private Rigidbody _rigidbody;
    private bool _isSelectedCharacter;
    private float _curHp;
    private int _curSkillGauge;
    private float _curAtk;
    private float _curDef;
    private float _curMoveSpeed;
    private float _curRunSpeed;
    private bool _wasGrounded;
    private float _currentAnimSpeed;

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

    public event Action<float> OnMoveSpeedChanged;
    public event Action<bool> OnGroundedChanged;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
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

        Debug.DrawRay(_groundCheckPoint.position, Vector3.down * _groundCheckDistance, Color.red);
    }
    public void Initialize(CharacterData data, StatData stats)
    {
        _data = data;
        _isSelectedCharacter = false;
        _curHp = stats.Hp;
        _curAtk = stats.Atk;
        _curDef = stats.Def;
        _curMoveSpeed = stats.MoveSpeed;
        _curRunSpeed = _curMoveSpeed * RunSpeedMultiplier;
        _curSkillGauge = 0;

        CharacterSkillSystem skillSystem = GetComponent<CharacterSkillSystem>();
        if (skillSystem != null)
        {
            skillSystem.Initialize(data);
        }
    }

    public void Move(Vector3 moveDirection, bool isRunning)
    {
        // Debug.Log($"Move 호출: {moveDirection}");
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

        if (_rigidbody.linearVelocity.y > JumpVelocityThreshold)
        {
            return false;
        }
        return Physics.Raycast(_groundCheckPoint.position, Vector3.down, _groundCheckDistance);
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
}
