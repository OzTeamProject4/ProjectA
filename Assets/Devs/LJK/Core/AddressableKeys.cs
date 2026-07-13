public static class AddressableKey
{
    public static class Prefab
    {
        public const string UILayer = "Prefab/UILayer";
        public const string AudioController = "Prefab/AudioView";
    }

    public static string GetUIKey(UIType uIType)
    {
        string key = $"UI/{uIType}";
        return key;
    }
}