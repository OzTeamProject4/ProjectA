public class StudentData : BaseData
{
    public string Name { get; init; }
    public int Star { get; init; }
    public string FullBodyKey { get; init; }
    public string PortraitKey { get; init; }
    public float BaseHp { get; init; }
    public float BaseAttack { get; init; }
    public float BaseDefense { get; init; }
    public float BaseMoveSpeed { get; init; }
    public ElementType ElementType { get; init; }

    public float HpGrow { get; init; }
    public float AtkGrow { get; init; }
    public float DefGrow { get; init; }
    public float MoveSpeedGrow { get; init; }



    //public string SkillList { get; init; }

    //public int SkillGauge { get; init; }


    
    //public string PrefabPath { get; init; }

    //public string Description { get; init; }

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