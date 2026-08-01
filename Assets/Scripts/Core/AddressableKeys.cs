public static class AddressableKey
{
    public static class Prefab
    {
        public const string UILayer = "Prefab/UILayer";
        public const string AudioView = "Prefab/AudioView";
        public const string ObjectPoolRoot = "Prefab/ObjectPoolRoot";
        public const string StageSelectMap01 = "Prefab/StageSelectMap_01";
        public const string StageEntry = "Prefab/StageEntry";
        public const string InventoryItemSlot = "Prefab/ItemSlot";
    }

    public static class Data
    {
        public const string StudentData = "Data/StudentData";
        public const string StudentGradeData = "Data/StudentGrade";
        public const string StudentLevelData = "Data/StudentLevel";
        public const string Item = "Data/Item";
        public const string Material = "Data/Material";
        public const string Currency = "Data/Currency";
        public const string Equipment = "Data/Equipment";
        public const string Signature = "Data/Signature";
        public const string Stage = "Data/Stage";
        public const string StageWave = "Data/StageWave";
        public const string CharacterSkill = "Data/StudentSkill";
        public const string Enemy = "Data/EnemyData";
        public const string EnemySkill = "Data/EnemySkillData";
        public const string MissionList = "Data/MissionList";
        public const string Audio = "Data/Audio";
        public const string StoryInfo = "Data/StoryInfo";
        public const string DialoguePortrait = "Data/DialoguePortrait";
        public const string StudentGachaList = "Data/StudentGachaList";
        public const string StudentGacha = "Data/StudentGacha";
    }

    public static class Asset
    {
        public const string LoadingVideoClip = "Video/Loading";
    }

    public static string GetUIKey(UIType uIType)
    {
        string key = $"UI/{uIType}";
        return key;
    }
}