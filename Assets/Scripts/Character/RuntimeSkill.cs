using UnityEngine;

public class RuntimeSkill
{
    private CharacterSkillData _data;
    private float _lastUsedTime;
    private GameObject _projectilePrefab;

    public CharacterSkillData Data
    {
        get 
        {
            return _data; 
        }
    }

    public GameObject ProjectilePrefab
    {
        get 
        {
            return _projectilePrefab;
        }
    }
    public RuntimeSkill(CharacterSkillData data)
    {
        _data = data;
        _lastUsedTime = -data.Cooldown;
    }

    public float CooldownProgress
    {
        get
        {
            if (_data.Cooldown <= 0)
            {
                return 1.0f;
            }

            else
            {
                return Mathf.Clamp01((Time.time - _lastUsedTime) / _data.Cooldown);
            }
        }
    }

    public void SetProjectilePrefab(GameObject prefab)
    {
        _projectilePrefab = prefab;
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
