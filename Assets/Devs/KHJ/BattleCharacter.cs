using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BattleCharacter : MonoBehaviour
{
    // TODO 희준 캐릭터 모델링시 수치 변화 필요
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _groundCheckDistance = 0.05f; 
    [SerializeField] private float _rotationSpeed = 4.0f;
    [SerializeField] private Transform _groundCheckPoint;
    


    private CharacterData _data;
    private Rigidbody _rigidbody;

    private bool _isSelectedCharacter;
    private int _curHp;
    private int _curSkillGauge;
    private int _curAtk;
    private int _curDef;
    private float _curMoveSpeed;
    private const float MoveThreshold = 0.1f;

    public string CharacterName => _data.Name;
    public int CurHp => _curHp;

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
        Debug.DrawRay(_groundCheckPoint.position, Vector3.down * _groundCheckDistance, Color.red);
    }
    public void Initialize(CharacterData data)
    {
        _data = data;
        _isSelectedCharacter = false;
        _curHp = _data.BaseHp;
        _curSkillGauge = 0;
        _curAtk = _data.BaseAtk;
        _curDef = _data.BaseDef;
        _curMoveSpeed = _data.BaseMoveSpeed;
    }

    public void Move(Vector3 moveDirection)
    {
        Vector3 velocity = moveDirection * _curMoveSpeed;
        velocity.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = velocity;

        if (moveDirection.magnitude > MoveThreshold)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }


    }

    public void Jump()
    {
        if (_groundCheckPoint == null)
        {
            Debug.LogError("_groundCehckPoint 확인");
            return;
        }

        bool isGround = Physics.Raycast(_groundCheckPoint.position, Vector3.down, _groundCheckDistance);
        if (isGround == false) 
        {
            return;
        }

        Vector3 velocity = _rigidbody.linearVelocity;
        velocity.y = _jumpForce;
        _rigidbody.linearVelocity = velocity;
    }
}
