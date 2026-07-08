using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BattleCharacter : MonoBehaviour
{
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _groundCheckDistance = 1.1f;

    private CharacterData _data;
    private Rigidbody _rigidbody;

    private bool _isSelectedCharacter;
    private int _curHp;
    private int _curSkillGauge;
    private int _curAtk;
    private int _curDef;
    private float _curMoveSpeed;

    public string CharacterName => _data.Name;
    public int CurHp => _curHp;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
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

    }

    public void Jump()
    {
        bool isGround = Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance);
        if (isGround == false) 
        {
            return;
        }

        Vector3 velocity = _rigidbody.linearVelocity;
        velocity.y = _jumpForce;
        _rigidbody.linearVelocity = velocity;
    }
}
