using System.Collections.Generic;

public class CharacterData : BaseData
{
    //Test
    public CharacterData(string id, string name, int star, string iconPath)
    {
        DataId = id;
        Name = name;
        Star = star;
        CharacterIconPath = iconPath;
    }

    public string Name { get; init; }
    public int Star { get; init; }
    public int Exp { get; init; }
    public string SkillList { get; init; }
    public ElementType Type { get; init; }
    public int SkillGauge { get; init; }

    public float Hp { get; init; }
    public float Atk { get; init; }
    public float Def { get; init; }
    public float MoveSpeed { get; init; }

    public float HpGrow { get; init; }
    public float AtkGrow { get; init; }
    public float DefGrow { get; init; }
    public float MoveSpeedGrow { get; init; }

    public string CharacterIconPath { get; init; }
    
    public string PrefabPath { get; init; }

    public string Description { get; init; }

    private List<string> _parsedSkillList;

    public List<string> ParsedSkillList
    {
        get
        {
            if (_parsedSkillList == null)
            {
                _parsedSkillList = new List<string>(Util.ParseIds(SkillList));
            }

            return _parsedSkillList;
        }
    }
}