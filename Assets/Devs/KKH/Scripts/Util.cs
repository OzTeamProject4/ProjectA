using System;
using System.Linq;

public static class Util
{
    public static string[] ParseIds(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return Array.Empty<string>();
        }

        return data.Split(',');
    }

    public static int[] ParseCounts(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return Array.Empty<int>();
        }

        return data.Split(',').Select(int.Parse).ToArray();
    }

    public static bool TryPairMaterials(string[] ids, int[] counts, out (string ItemId, int Count)[] materials)
    {
        if (ids.Length == 0 || ids.Length != counts.Length)
        {
            materials = Array.Empty<(string, int)>();
            return false;
        }

        materials = new (string, int)[ids.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            materials[i] = (ids[i], counts[i]);
        }

        return true;
    }
}