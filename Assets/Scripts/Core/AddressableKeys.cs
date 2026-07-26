public static class AddressableKey
{
    public static class Prefab
    {
        public const string UILayer = "Prefab/UILayer";
        public const string AudioView = "Prefab/AudioView";
        public const string ObjectPoolRoot = "Prefab/ObjectPoolRoot";
        public const string StageSelectMap01 = "Prefab/StageSelectMap_01";
        public const string StageEntry = "Prefab/StageEntry";
    }

    public static class Data
    {
        public const string StudentData = "Data/StudentData";
        public const string StudentGradeData = "Data/StudentGradeData";
        public const string StudentLevelData = "Data/StudentLevelData";
        public const string Item = "Data/Item";
        public const string Currency = "Data/Currency";
        public const string Equipment = "Data/Equipment";
        public const string Signature = "Data/Signature";
        public const string Stage = "Data/Stage";
        public const string StageWave = "Data/StageWave";
        public const string CharacterSkill = "Data/StudentSkillData";
        public const string Enemy = "Data/EnemyData";
        public const string EnemySkill = "Data/EnemySkillData";
        
    }

    public static class Asset
    {
        public const string LoadingVideoClip = "Video/Test";
    }

    public static string GetUIKey(UIType uIType)
    {
        string key = $"UI/{uIType}";
        return key;
    }
}