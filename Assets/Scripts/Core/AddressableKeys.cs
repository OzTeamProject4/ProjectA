public static class AddressableKey
{
    public static class Prefab
    {
        public const string UILayer = "Prefab/UILayer";
        public const string AudioController = "Prefab/AudioView";
    }

    public static class Data
    {
        public const string CharacterData = "Data/CharacterData";
        public const string CharacterGrade = "Data/CharacterGrade";
        public const string LevelExp = "Data/LevelExp";
        public const string Item = "Data/Item";
        public const string Equipment = "Data/Equipment";
        public const string Signature = "Data/Signature";
    }

    public static string GetUIKey(UIType uIType)
    {
        string key = $"UI/{uIType}";
        return key;
    }
}