using System;
using System.Linq;
using System.Text.RegularExpressions;

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

    public static int ParseMaterialTier(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return 0;
        }

        Match match = Regex.Match(itemId, @"T(\d+)$");
        if (!match.Success)
        {
            return 0;
        }

        return int.Parse(match.Groups[1].Value);
    }
}