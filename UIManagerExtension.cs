using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public static class UIManagerExtension
{
    public static async UniTask OpenStudentManagementListAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenOverlayAsync();

        try
        {
            uiManager.CloseLoading();
            await uiManager.OpenContentRootAsync(UIType.StudentManagementList, cancellationToken);
        }
        finally
        {
            uiManager.CloseOverlay();
        }
    }

    public static void CloseStudentManagementList(this UIManager uiManager)
    {
        uiManager.Close(UIType.StudentManagementList);
    }

    public static async UniTask OpenStudentManagementAsync(this UIManager uiManager, StudentModel studentModel, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenOverlayAsync();

        try
        {
            BaseUI baseUI = await uiManager.OpenContentRootAsync(UIType.StudentManagement, cancellationToken);

            if (baseUI is not StudentManagementView studentManagementView)
            {
                Debug.LogError("StudentManagementView 타입이 아닙니다.");
                return;
            }

            studentManagementView.SetModel(studentModel);
        }
        finally
        {
            uiManager.CloseOverlay();
        }
    }

    public static void CloseStudentManagement(this UIManager uiManager)
    {
        uiManager.Close(UIType.StudentManagement);
    }

    //========================================================
    //TODO 임시 구현. 스택 방식으로 UIManager 리팩토링시 제거
    private static readonly UIType[] StudentManagementPopupTypes =
    {
        UIType.ExperienceInventoryPopup,
        UIType.EquipmentInventoryPopup,
        UIType.EquipmentCraftPopup,
        UIType.EquipmentInfoPopup,
        UIType.CraftEquipmentInfoPopup
    };

    
    public static void CloseStudentManagementPopups(this UIManager uiManager)
    {
        foreach (UIType uiType in StudentManagementPopupTypes)
        {
            uiManager.Close(uiType);
        }
    }
    //========================================================

    public static async UniTask OpenExperienceInventoryPopupAsync(this UIManager uiManager, StudentModel studentModel, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenOverlayRootAsync(UIType.ExperienceInventoryPopup, cancellationToken);

        if (baseUI is not ExperienceInventoryPopupView experienceInventoryPopupView)
        {
            Debug.LogError("ExperienceInventoryPopupView 타입이 아닙니다.");
            return;
        }

        experienceInventoryPopupView.SetModel(studentModel);
    }

    public static void CloseExperienceInventoryPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.ExperienceInventoryPopup);
    }

    public static async UniTask OpenEquipmentInventoryPopupAsync(this UIManager uiManager, EquipType equipType, StudentModel studentModel, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.EquipmentInventoryPopup, cancellationToken);

        if (baseUI is not EquipmentInventoryPopupView equipmentInventoryPopupView)
        {
            Debug.LogError("EquipmentInventoryPopupView 타입이 아닙니다.");
            return;
        }

        equipmentInventoryPopupView.SetModel(equipType, studentModel);
    }

    public static void CloseEquipmentInventoryPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.EquipmentInventoryPopup);
    }

    public static async UniTask OpenEquipmentCraftPopupAsync(this UIManager uiManager, EquipType equipType, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.EquipmentCraftPopup, cancellationToken);

        if (baseUI is not EquipmentCraftPopupView equipmentCraftPopupView)
        {
            Debug.LogError("EquipmentCraftPopupView 타입이 아닙니다.");
            return;
        }

        equipmentCraftPopupView.SetEquipType(equipType);
    }

    public static void CloseEquipmentCraftPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.EquipmentCraftPopup);
    }

    public static async UniTask OpenEquipmentInfoPopupAsync(this UIManager uiManager, StudentModel studentModel, EquipmentModel equipmentModel, Vector3 position, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenOverlayRootAsync(UIType.EquipmentInfoPopup, cancellationToken);

        if (baseUI is EquipmentInfoPopupView equipmentInfoPopupView)
        {
            equipmentInfoPopupView.SetModel(equipmentModel, studentModel, position);
        }
    }

    public static void CloseEquipmentInfoPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.EquipmentInfoPopup);
    }

    public static async UniTask OpenCraftEquipmentInfoPopupAsync(this UIManager uiManager, EquipmentCraftModel equipmentCraftModel, Vector3 position, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenOverlayRootAsync(UIType.CraftEquipmentInfoPopup, cancellationToken);

        if (baseUI is not CraftEquipmentInfoPopupView craftEquipmentInfoPopupView)
        {
            Debug.LogError("CraftEquipmentInfoPopupView 타입이 아닙니다.");
            return;
        }

        craftEquipmentInfoPopupView.SetModel(equipmentCraftModel, position);
    }

    public static void CraftEquipmentInfoPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.CraftEquipmentInfoPopup);
    }
    public static async UniTask<StageInfoPopupView> OpenStageInfoPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.StageInfoPopup, cancellationToken);
        return GetView<StageInfoPopupView>(baseUI, UIType.StageInfoPopup);
    }

    public static void CloseStageInfoPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.StageInfoPopup);
    }

    public static async UniTask OpenOverlayAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenOverlayRootAsync(UIType.Overlay, cancellationToken);
    }

    public static void CloseOverlay(this UIManager uiManager)
    {
        uiManager.Close(UIType.Overlay);
    }

    public static async UniTask OpenLoadingAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenTestRootAsync(UIType.Loading, cancellationToken);

        if (baseUI is LoadingUI loadingUI)
        {
            await loadingUI.WaitUntilInitializedAsync();
        }
    }

    public static void CloseLoading(this UIManager uiManager)
    {
        uiManager.Close(UIType.Loading);
    }

    public static async UniTask OpenPracticeFieldScreenAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.PracticeFieldScreen, cancellationToken);
    }

    public static void ClosePracticeFieldScreen(this UIManager uiManager)
    {
        uiManager.Close(UIType.PracticeFieldScreen);
    }

    public static async UniTask OpenStageSelectScreenAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.StageSelectScreen, cancellationToken);
    }

    public static void CloseStageSelectScreen(this UIManager uiManager)
    {
        uiManager.Close(UIType.StageSelectScreen);
    }

    public static async UniTask<PartySelectPopupView> OpenPartySelectPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.PartySelectPopup, cancellationToken);
        return GetView<PartySelectPopupView>(baseUI, UIType.PartySelectPopup);
    }

    public static void ClosePartySelectPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.PartySelectPopup);
    }

    public static async UniTask OpenDictionaryScreenAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.DictionaryScreen, cancellationToken);
    }

    public static void CloseDictionaryScreen(this UIManager uiManager)
    {
        uiManager.Close(UIType.DictionaryScreen);
    }

    public static async UniTask OpenSettingPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.DictionaryScreen, cancellationToken);
    }

    public static void CloseSettingPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.SettingPopup);
    }

    public static async UniTask OpenFarmingDungeonScreenAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.FarmingDungeonScreen, cancellationToken);
    }

    public static void CloseFarmingDungeonScreen(this UIManager uiManager)
    {
        uiManager.Close(UIType.FarmingDungeonScreen);
    }

    public static async UniTask<BattleResultPopupView> OpenBattleResultAsync(this UIManager uiManager, bool isVictory, string stageId, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.BattleResultPopup, cancellationToken);
        return GetView<BattleResultPopupView>(baseUI, UIType.BattleResultPopup);
    }

    public static void CloseBattleResult(this UIManager uiManager)
    {
        uiManager.Close(UIType.BattleResultPopup);
    }

    public static async UniTask<BattlePausePopupView> OpenBattlePauseAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.BattlePausePopup, cancellationToken);
        return GetView<BattlePausePopupView>(baseUI, UIType.BattlePausePopup);
    }

    public static void CloseBattlePause(this UIManager uiManager)
    {
        uiManager.Close(UIType.BattlePausePopup);
    }

    public static async UniTask<ReturnToLobbyPopupView> OpenReturnToLobbyPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.ReturnToLobbyPopup, cancellationToken);
        return GetView<ReturnToLobbyPopupView>(baseUI, UIType.ReturnToLobbyPopup);
    }

    public static void CloseReturnToLobbyPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.ReturnToLobbyPopup);
    }

    public static async UniTask<StageSelectHudView> OpenStageSelectHudAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenTestRootAsync(UIType.StageSelectHud, cancellationToken);
        GameManager.Instance.AudioManager.PlayBGM("BGM_Stage");
        return GetView<StageSelectHudView>(baseUI, UIType.StageSelectHud);
    }

    public static void CloseStageSelectHud(this UIManager uiManager)
    {
        uiManager.Close(UIType.StageSelectHud);
    }

    public static async UniTask OpenMissionScreenAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.MissionScreen, cancellationToken);
    }

    public static void CloseMissionScreen(this UIManager uiManager)
    {
        uiManager.Close(UIType.MissionScreen);
    }

    public static async UniTask OpenInventoryScreenAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.InventoryScreen, cancellationToken);
    }

    public static void CloseInventoryScreen(this UIManager uiManager)
    {
        uiManager.Close(UIType.InventoryScreen);
    }

    public static async UniTask OpenInventoryDetailAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenOverlayAsync();

        try
        {
            await uiManager.OpenContentRootAsync(UIType.InventoryDetail, cancellationToken);
        }
        finally
        {
            uiManager.CloseOverlay();
        }
    }

    public static void CloseInventoryDetail(this UIManager uiManager)
    {
        uiManager.Close(UIType.InventoryDetail);
    }

    public static async UniTask OpenAchievementScreenAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.AchievementScreen, cancellationToken);
    }

    public static void CloseAchievementScreen(this UIManager uiManager)
    {
        uiManager.Close(UIType.AchievementScreen);
    }

    public static async UniTask OpenLobbyAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await GameManager.Instance.UIManager.OpenOverlayAsync();
        GameManager.Instance.UIManager.CloseLoading();
        await uiManager.OpenTestRootAsync(UIType.Lobby, cancellationToken);
        GameManager.Instance.AudioManager.PlayBGM("BGM_Lobby_01");
        GameManager.Instance.UIManager.CloseOverlay();
    }

    public static async UniTask<BattleHUDView> OpenBattleHUDAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        GameManager.Instance.AudioManager.PlayBGM("BGM_Battle");
        BaseUI baseUI = await uiManager.OpenContentRootAsync(UIType.BattleHUD, cancellationToken);
        return GetView<BattleHUDView>(baseUI, UIType.BattleHUD);
    }

    public static void CloseBattleHUD(this UIManager uiManager)
    {
        uiManager.Close(UIType.BattleHUD);
    }

    public static void CloseLobby(this UIManager uiManager)
    {
        uiManager.Close(UIType.Lobby);
    }

    public static async UniTask<EnemyHud> OpenEnemyHudUI(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenTestRootAsync(UIType.EnemyHud, cancellationToken);
        return GetView<EnemyHud>(baseUI, UIType.EnemyHud);
    }

    public static void CloseEnemyHudUI(this UIManager uiManager)
    {
        uiManager.Close(UIType.EnemyHud);
    }

    public static async UniTask<ProfileView> OpenProfileAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenOverlayAsync();

        try
        {
            BaseUI baseUI = await uiManager.OpenContentRootAsync(UIType.Profile, cancellationToken);

            return GetView<ProfileView>(baseUI, UIType.Profile);
        }
        finally
        {
            uiManager.CloseOverlay();
        }
    }

    public static void CloseProfile(this UIManager uiManager)
    {
        uiManager.Close(UIType.Profile);
    }

    public static async UniTask<ProfileStudentSelectPopupView> OpenProfileStudentSelectPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.ProfileStudentSelectPopup, cancellationToken);

        return GetView<ProfileStudentSelectPopupView>(baseUI, UIType.ProfileStudentSelectPopup);
    }

    public static void CloseProfileStudentSelectPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.ProfileStudentSelectPopup);
    }

    private static T GetView<T>(BaseUI baseUI, UIType uiType) where T : BaseUI
    {
        if (baseUI == null)
        {
            return null;
        }

        if (baseUI is not T view)
        {
            return null;
        }

        return view;
    }

    public static async UniTask OpenDialogueAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        GameManager.Instance.AudioManager.PlayBGM("BGM_Dialogue");
        BaseUI DialogueUI = await uiManager.OpenTestRootAsync(UIType.Dialogue, cancellationToken);

        if (DialogueUI is DialogueView dialogueView)
        {
            await dialogueView.WaitUntilInitializedAsync();
        }
    }

    public static void CloseDialogue(this UIManager uiManager)
    {
        uiManager.Close(UIType.Dialogue);
    }

    public static async UniTask OpenDialogueHistoryAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.DialogueHistory, cancellationToken);
    }

    public static void CloseDialogueHistory(this UIManager uiManager)
    {
        uiManager.Close(UIType.DialogueHistory);
    }

    public static async UniTask OpenStudentGachaAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        GameManager.Instance.AudioManager.PlayBGM("BGM_Gacha");
        await uiManager.OpenTestRootAsync(UIType.StudentGacha, cancellationToken);
    }

    public static void CloseStudentGacha(this UIManager uiManager)
    {
        GameManager.Instance.AudioManager.PlayBGM("BGM_Lobby_01");
        uiManager.Close(UIType.StudentGacha);   
    }
}