using System.Collections.Generic;
using UnityEngine;

public class BattleCharacter : MonoBehaviour
{
    private CharacterData _data;

    private bool _isSelectedCharacter;
    private int _curHp;
    private int _curSkillGauge;
    private int _curAtk;
    private int _curDef;
    private float _curMoveSpeed;

    public string CharacterName => _data.Name;
    public int CurHp => _curHp;

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
        transform.position = transform.position + (moveDirection * _curMoveSpeed * Time.deltaTime);
    }
}
