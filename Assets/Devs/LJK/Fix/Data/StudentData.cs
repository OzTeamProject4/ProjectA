public class StudentData : BaseData
{
    public string Name { get; init; }
    public int Star { get; init; }
    public ElementType Type { get; init; }

    // 1레벨 기준 스탯
    public float MaxHp { get; init; }
    public float Attack { get; init; }
    public float Defence { get; init; }
    public float MoveSpeed { get; init; }

    // 레벨당 상승량
    public float HpGrow { get; init; }
    public float AtkGrow { get; init; }
    public float DefGrow { get; init; }
    public float MoveSpeedGrow { get; init; }

    public string CharacterIconPath { get; init; }
    public string PrefabPath { get; init; }

    // 스킬 탭에서 사용 예정
    //public string SkillList { get; init; }

    //public int SkillGauge { get; init; }

    //private List<string> _parsedSkillList;

    //public List<string> ParsedSkillList
    //{
    //    get
    //    {
    //        if (_parsedSkillList == null)
    //        {
    //            _parsedSkillList = new List<string>(Util.ParseIds(SkillList));
    //        }

    //        return _parsedSkillList;
    //    }
    //}
}