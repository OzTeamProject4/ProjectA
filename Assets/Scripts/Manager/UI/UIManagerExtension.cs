using Cysharp.Threading.Tasks;
using System.Threading;

public static class UIManagerExtension
{
    public static async UniTask<StageInfoPopupView> OpenStageInfoPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.StageInfoPopup, cancellationToken);

        return GetView<StageInfoPopupView>(baseUI, UIType.StageInfoPopup);
    }

    public static void CloseStageInfoPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.StageInfoPopup);
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

    public static async UniTask OpenBattleResultAsync(this UIManager uiManager, bool isVictory, string stageId, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.BattleResultPopup, cancellationToken);
        BattleResultPopupView view = GetView<BattleResultPopupView>(baseUI, UIType.BattleResultPopup);

        if (null == view)
        {
            return;
        }

        await view.WaitForReturnAsync(isVictory, stageId);

        uiManager.Close(UIType.BattleResultPopup);
    }

    public static async UniTask<BattlePauseChoice> OpenBattlePauseAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.BattlePausePopup, cancellationToken);
        BattlePausePopupView view = GetView<BattlePausePopupView>(baseUI, UIType.BattlePausePopup);

        if (null == view)
        {
            return BattlePauseChoice.Resume;
        }

        BattlePauseChoice choice = await view.WaitForChoiceAsync();

        uiManager.Close(UIType.BattlePausePopup);

        return choice;
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
        await uiManager.OpenTestRootAsync( UIType.InventoryScreen, cancellationToken);
    }

    public static void CloseInventoryScreen(this UIManager uiManager)
    {
        uiManager.Close(UIType.InventoryScreen);
    }

    public static async UniTask OpenAchievementScreenAsync(this UIManager uiManager,CancellationToken cancellationToken = default)
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