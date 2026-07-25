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
        await uiManager.OpenTestRootAsync(UIType.Loading, cancellationToken);
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

    public static async UniTask OpenFarmingDungeonScreenAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.FarmingDungeonScreen, cancellationToken);
    }

    public static void CloseFarmingDungeonScreen(this UIManager uiManager)
    {
        uiManager.Close(UIType.FarmingDungeonScreen);
    }

    public static async UniTask OpenCharacterGachaScreenAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.CharacterGachaScreen, cancellationToken);
    }

    public static void CloseCharacterGachaScreen(this UIManager uiManager)
    {
        uiManager.Close(UIType.CharacterGachaScreen);
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
        GameManager.Instance.UIManager.CloseOverlay();
    }

    public static async UniTask<BattleHUDView> OpenBattleHUDAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
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
}