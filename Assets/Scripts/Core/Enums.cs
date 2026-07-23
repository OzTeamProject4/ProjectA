public enum UIType
{
    Lobby,
    CharacterList,
    CharacterDetail,
    ExpItemSelectPopup,
    CraftPopup,
    EquipmentListPopup,
    EquipmentDetailPopup,
    ItemPreviewPopup,
    StageInfoPopup,
    PartySetupPopup,
    Overlay,
    Loading,

    PracticeFieldScreen,
    StageSelectScreen,
    DictionaryScreen,
    FarmingDungeonScreen,
    CharacterGachaScreen,
    MissionScreen,
    InventoryScreen,
    AchievementScreen
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
    LoadStageData,
    LoadStageWaveData,
    Complete,
    LoadSkillData,
    ItemPreviewPopup,
    StageInfoPopup,
    PartySetupPopup
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

public enum ScreenType
{
    Lobby,
    StageSelect,
    Battle
}
