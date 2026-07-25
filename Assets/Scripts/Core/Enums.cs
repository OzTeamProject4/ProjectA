public enum UIType
{
    Lobby,
    StudentManagementList,
    StudentManagement,
    ExperienceInventoryPopup,
    EquipmentInventoryPopup,
    EquipmentCraftPopup,
    EquipmentInfoPopup,
    CraftEquipmentInfoPopup,
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
    AchievementScreen,
    PartySelectPopup,
    BattleResultPopup,
    BattlePausePopup,
    BattleHUD
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
    LoadStudentData,
    LoadStudentGradeData,
    LoadStudentLevelData,
    LoadItemData,
    LoadCurrencyData,
    LoadEquipmentData,
    LoadSignatureData,
    LoadStageData,
    LoadStageWaveData,
    Complete,
    LoadSkillData,
    ItemPreviewPopup,
    StageInfoPopup,
    PartySetupPopup,
    LoadEnemyData,
    LoadEnemySkillData
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
    Currency,
    Equipment,
    Signature,
    Material
}

public enum CurrencyType
{
    Gold,
    Crystal,
    ExpBook,
    Ticket,
    Material
}

public enum EquipType
{
    Weapon,
    Hat,
    Armor,
    Boots,
    Accessory,
    Signature
}

public enum ScreenType
{
    Lobby,
    StageSelect,
    Battle
}