public enum UIType
{
    CharacterList,
    CharacterDetail,
    ExpItemSelectPopup,
    CraftPopup,
    EquipmentListPopup,
    EquipmentDetailPopup,
    ItemPreviewPopup,
    Overlay,
    Loading
}

public enum LoadingState
{
    Loading,
    Ready
}

public enum LoadingStep
{
    None,
    Initialize,
    LoadCharacterData,
    LoadCharacterGradeData,
    LoadLevelExpData,
    LoadItemData,
    LoadEquipmentData,
    LoadSignatureData,
    Complete,
    LoadSkillData
}

public enum UIRoot
{
    A,
    B,
    C,
    D,
    E
}

public enum ElementType
{
    Normal,
    Fire,
    Water,
    Grass,
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

public enum StatType
{
    MaxHp,
    Atk,
    Def,
    MoveSpeed
}
