public static class AddressableKey
{
    public static class Prefab
    {
        public const string UILayer = "Prefab/UILayer";
        public const string AudioView = "Prefab/AudioView";
        public const string ObjectPoolRoot = "Prefab/ObjectPoolRoot";
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