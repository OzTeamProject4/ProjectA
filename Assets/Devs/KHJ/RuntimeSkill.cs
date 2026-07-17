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
