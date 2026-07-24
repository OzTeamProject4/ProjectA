public enum UIType
{
    Lobby,
    CharacterList,
    CharacterDetail,
    InventoryDetail,
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
    AchievementScreen,
    PartySelectPopup,
    BattleResultPopup,
    BattlePausePopup,
    ReturnToLobbyPopup,
    StageSelectHud
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
    LoadCurrencyData,
    LoadEquipmentData,
    LoadSignatureData,
    LoadStageData,
    LoadStageWaveData,
    Complete,
    LoadSkillData,
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
    Signature
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

public enum StatType
{
    MaxHp,
    Attack,
    Defence,
    MoveSpeed
}

public enum ScreenType
{
    Lobby,
    StageSelect,
    Battle
}
