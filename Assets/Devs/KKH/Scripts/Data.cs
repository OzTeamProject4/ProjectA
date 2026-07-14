using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public static class DataKeys
{
    public const string CharacterData = "Data/CharacterData";
    public const string CharacterGrade = "Data/CharacterGrade";
    public const string LevelExp = "Data/LevelExp";
    public const string Item = "Data/Item";
    public const string Equipment = "Data/Equipment";
    public const string Signature = "Data/Signature";
}

public enum ElementType
{
    Fire,
    Water,
    Thunder,
    Light,
    Dark
}

public enum RollType
{
    Attacker,
    Buffer
}

public enum ItemType
{
    ExpBook,
    Ticket,
    Material
}

public enum EquipType
{
    Weapon,
    Helmet,
    Armor,
    Greeve,
    Accessory,
    Signature
}

public class ItemData : BaseData
{
    public string Name { get; init; }
    public ItemType Type { get; init; }
    public int Gold { get; init; }
    public int Crystal { get; init; }
    public int Value { get; init; }
    public string SpritePath { get; init; }
}

public class CharacterData : BaseData
{
    public string Name { get; init; }
    public int Star { get; init; }
    public int Level { get; init; }
    public string SkillList { get; init; }
    public ElementType Type { get; init; }
    public int SkillGauge { get; init; }

    public float Hp { get; init; }
    public float Atk { get; init; }
    public float Def { get; init; }
    public float AtkSpeed { get; init; }
    public float MoveSpeed { get; init; }

    public float HpGrow { get; init; }
    public float AtkGrow { get; init; }
    public float DefGrow { get; init; }
    public float AtkSpeedGrow { get; init; }
    public float MoveSpeedGrow { get; init; }

    public string CharacterIconPath { get; init; }
    public string PrefabPath { get; init; }
    public string Description { get; init; }

    private List<string> _parsedSkillList;
    public List<string> ParsedSkillList
    {
        get
        {
            if (_parsedSkillList == null)
            {
                _parsedSkillList = new List<string>(Util.ParseIds(SkillList));
            }

            return _parsedSkillList;
        }
    }
}

public class CharacterGradeData : BaseData
{
    public int Star { get; init; }
    public int MaxLevel { get; init; }
    public int RequiredToNext { get; init; }

    public float HpGrow { get; init; }
    public float AtkGrow { get; init; }
    public float DefGrow { get; init; }
    public float AtkSpeedGrow { get; init; }
    public float MoveSpeedGrow { get; init; }
}

public class LevelExpData : BaseData
{
    public int Level { get; init; }
    public int RequiredExp { get; init; }
}

public class EquipmentData : BaseData
{
    public string Name { get; init; }
    public EquipType Type { get; init; }
    public string AllowedId { get; init; }
    public int Gold { get; init; }
    public string RequiredItemId { get; init; }
    public string RequiredItemCount { get; init; }
    public float BonusRate { get; init; }
    public float MaxHp { get; init; }
    public float Atk { get; init; }
    public float Def { get; init; }
    public float AtkSpeed { get; init; }
    public float MoveSpeed { get; init; }
    public string SpritePath { get; init; }
    public string Description { get; init; }

    private string[] _requiredItemIds;
    private int[] _requiredItemCounts;

    public string[] RequiredItemIds
    {
        get
        {
            if (_requiredItemIds == null)
            {
                _requiredItemIds = Util.ParseIds(RequiredItemId);
            }

            return _requiredItemIds;
        }
    }

    public int[] RequiredItemCounts
    {
        get
        {
            if (_requiredItemCounts == null)
            {
                _requiredItemCounts = Util.ParseCounts(RequiredItemCount);
            }

            return _requiredItemCounts;
        }
    }

    public bool TryGetRequiredMaterials(out (string ItemId, int Count)[] materials)
    {
        if (!Util.TryPairMaterials(RequiredItemIds, RequiredItemCounts, out materials))
        {
            return false;
        }

        return true;
    }
}

public class SignatureData : BaseData
{
    public string SignatureId { get; init; }
    public int EnchantLevel { get; init; }
    public float MaxHp { get; init; }
    public float Atk { get; init; }
    public float Def { get; init; }
    public float AtkSpeed { get; init; }
    public float MoveSpeed { get; init; }
    public string RequiredItemId { get; init; }
    public string RequiredItemCount { get; init; }
    public int RequiredGold { get; init; }

    private string[] _requiredItemIds;
    private int[] _requiredItemCounts;

    public string[] RequiredItemIds
    {
        get
        {
            if (_requiredItemIds == null)
            {
                _requiredItemIds = Util.ParseIds(RequiredItemId);
            }

            return _requiredItemIds;
        }
    }

    public int[] RequiredItemCounts
    {
        get
        {
            if (_requiredItemCounts == null)
            {
                _requiredItemCounts = Util.ParseCounts(RequiredItemCount);
            }

            return _requiredItemCounts;
        }
    }

    public bool TryGetRequiredMaterials(out (string ItemId, int Count)[] materials)
    {
        if (!Util.TryPairMaterials(RequiredItemIds, RequiredItemCounts, out materials))
        {
            return false;
        }

        return true;
    }
}