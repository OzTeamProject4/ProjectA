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
    InventoryDetail,
    AchievementScreen,
    PartySelectPopup,
    BattleResultPopup,
    BattlePausePopup,
    ReturnToLobbyPopup,
    StageSelectHud,
    BattleHUD,
    EnemyHud,   
    Dialogue,
    DialogueHistory
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
    LoadMaterialData,
    LoadCurrencyData,
    LoadEquipmentData,
    LoadSignatureData,
    LoadStageData,
    LoadStageWaveData,
    LoadSkillData,
    LoadEnemyData,
    LoadEnemySkillData,
    LoadMissionData,
    LoadAudioData,
    LoadStoryInfoData,
    LoadDialoguePortraitData,
    Complete
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
    Material,
    Shard
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

public enum InventoryFilterType
{
    All,
    Equipment,
    Currency,
    Material
}