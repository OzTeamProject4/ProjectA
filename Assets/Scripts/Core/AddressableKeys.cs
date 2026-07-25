public static class AddressableKey
{
    public static class Prefab
    {
        public const string UILayer = "Prefab/UILayer";
        public const string AudioView = "Prefab/AudioView";
        public const string ObjectPoolRoot = "Prefab/ObjectPoolRoot";
    }

    //TODO 어드레서블 경로 수정
    public static class Data
    {
        public const string StudentData = "Data/CharacterData";
        public const string StudentGradeData = "Data/CharacterGrade";
        public const string StudentLevelData = "Data/LevelExp";
        public const string Item = "Data/Item";
        public const string Equipment = "Data/Equipment";
        public const string Signature = "Data/Signature";
        public const string CharacterSkill = "Data/CharacterSkill";
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