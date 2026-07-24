using Cysharp.Threading.Tasks;
using System.Threading;

public static class UIManagerExtension
{
    //public static async UniTask<CharacterListView> OpenCharacterListAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    //{
    //    BaseUI baseUI = await uiManager.OpenContentRootAsync(UIType.CharacterList, cancellationToken);
    //    return GetView<CharacterListView>(baseUI, UIType.CharacterList);
    //}

    //public static void CloseCharacterList(this UIManager uiManager)
    //{
    //    uiManager.Close(UIType.CharacterList);
    //}

    //public static async UniTask<CharacterDetailView> OpenCharacterDetailAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    //{
    //    BaseUI baseUI = await uiManager.OpenContentRootAsync(UIType.CharacterDetail, cancellationToken);
    //    return GetView<CharacterDetailView>(baseUI, UIType.CharacterDetail);
    //}

    //public static void CloseCharacterDetail(this UIManager uiManager)
    //{
    //    uiManager.Close(UIType.CharacterDetail);
    //}

    //public static async UniTask<ExpItemSelectPopupView> OpenExpItemSelectPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    //{
    //    BaseUI baseUI = await uiManager.OpenOverlayRootAsync(UIType.ExpItemSelectPopup, cancellationToken);
    //    return GetView<ExpItemSelectPopupView>(baseUI, UIType.ExpItemSelectPopup);
    //}

    //public static void CloseExpItemSelectPopup(this UIManager uiManager)
    //{
    //    uiManager.Close(UIType.ExpItemSelectPopup);
    //}

    //public static async UniTask<CraftPopupView> OpenCraftPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    //{
    //    BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.CraftPopup, cancellationToken);
    //    return GetView<CraftPopupView>(baseUI, UIType.CraftPopup);
    //}

    //public static void CloseCraftPopup(this UIManager uiManager)
    //{
    //    uiManager.Close(UIType.CraftPopup);
    //}

    //public static async UniTask<EquipmentListPopupView> OpenEquipmentListPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    //{
    //    BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.EquipmentListPopup, cancellationToken);
    //    return GetView<EquipmentListPopupView>(baseUI, UIType.EquipmentListPopup);
    //}

    //public static void CloseEquipmentListPopup(this UIManager uiManager)
    //{
    //    uiManager.Close(UIType.EquipmentListPopup);
    //}

    //public static async UniTask<EquipmentDetailPopupView> OpenEquipmentDetailPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    //{
    //    BaseUI baseUI = await uiManager.OpenOverlayRootAsync(UIType.EquipmentDetailPopup, cancellationToken);
    //    return GetView<EquipmentDetailPopupView>(baseUI, UIType.EquipmentDetailPopup);
    //}

    //public static void CloseEquipmentDetailPopup(this UIManager uiManager)
    //{
    //    uiManager.Close(UIType.EquipmentDetailPopup);
    //}

    //public static async UniTask<ItemPreviewPopupView> OpenItemPreviewPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    //{
    //    BaseUI baseUI = await uiManager.OpenOverlayRootAsync(UIType.ItemPreviewPopup, cancellationToken);
    //    return GetView<ItemPreviewPopupView>(baseUI, UIType.ItemPreviewPopup);
    //}

    //public static void CloseItemPreviewPopup(this UIManager uiManager)
    //{
    //    uiManager.Close(UIType.ItemPreviewPopup);
    //}

    public static async UniTask<StageInfoPopupView> OpenStageInfoPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.StageInfoPopup, cancellationToken);
        return GetView<StageInfoPopupView>(baseUI, UIType.StageInfoPopup);
    }

    public static void CloseStageInfoPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.StageInfoPopup);
    }

    public static async UniTask OpenOverlayUIAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenOverlayRootAsync(UIType.Overlay, cancellationToken);
    }

    public static void CloseOverlayUI(this UIManager uiManager)
    {
        uiManager.Close(UIType.Overlay);
    }

    public static async UniTask OpenLoadingUIAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.Loading, cancellationToken);
    }

    public static void CloseLoadingUI(this UIManager uiManager)
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
        await uiManager.OpenTestRootAsync(UIType.Lobby, cancellationToken);
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