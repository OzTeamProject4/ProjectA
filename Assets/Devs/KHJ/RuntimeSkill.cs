using UnityEngine;

public class RuntimeSkill
{
    private CharacterSkillData _data;
    private float _lastUsedTime;

    public CharacterSkillData Data
    {
        get { return _data; }
    }

    public RuntimeSkill(CharacterSkillData data)
    {
        _data = data;
        _lastUsedTime = -data.Cooldown;
    }

    public bool IsReady()
    {
        return Time.time - _lastUsedTime >= _data.Cooldown;
    }

    public void MarkUsed()
    {
        _lastUsedTime = Time.time;
    }
   
}
