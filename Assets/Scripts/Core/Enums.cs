public enum UIType
{
    StudentManagementList,
    StudentManagement,
    ExperienceInventoryPopup,
    EquipmentInventoryPopup,
    EquipmentInfoPopup,
    EquipmentCraftPopup,
    CraftEquipmentInfoPopup,
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
    LoadStudentData,
    LoadStudentGradeData,
    LoadStudentLevelData,
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