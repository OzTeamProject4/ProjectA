using System.Threading;
using Cysharp.Threading.Tasks;

public static class UIManagerExtension
{
    public static async UniTask OpenTestUIAsync(
        this UIManager uiManager,
        CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(
            UIType.Test,
            cancellationToken);
    }

    public static void CloseTestUI(this UIManager uiManager)
    {
        uiManager.Close(UIType.Test);
    }

    public static async UniTask OpenPracticeFieldScreenAsync(
        this UIManager uiManager,
        CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(
            UIType.PracticeFieldScreen,
            cancellationToken);
    }

    public static void ClosePracticeFieldScreen(
        this UIManager uiManager)
    {
        uiManager.Close(UIType.PracticeFieldScreen);
    }

    public static async UniTask OpenStageSelectScreenAsync(
        this UIManager uiManager,
        CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(
            UIType.StageSelectScreen,
            cancellationToken);
    }

    public static void CloseStageSelectScreen(
        this UIManager uiManager)
    {
        uiManager.Close(UIType.StageSelectScreen);
    }

    public static async UniTask OpenDictionaryScreenAsync(
        this UIManager uiManager,
        CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(
            UIType.DictionaryScreen,
            cancellationToken);
    }

    public static void CloseDictionaryScreen(
        this UIManager uiManager)
    {
        uiManager.Close(UIType.DictionaryScreen);
    }

    public static async UniTask OpenFarmingDungeonScreenAsync(
        this UIManager uiManager,
        CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(
            UIType.FarmingDungeonScreen,
            cancellationToken);
    }

    public static void CloseFarmingDungeonScreen(
        this UIManager uiManager)
    {
        uiManager.Close(UIType.FarmingDungeonScreen);
    }

    public static async UniTask OpenCharacterGachaScreenAsync(
        this UIManager uiManager,
        CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(
            UIType.CharacterGachaScreen,
            cancellationToken);
    }

    public static void CloseCharacterGachaScreen(
        this UIManager uiManager)
    {
        uiManager.Close(UIType.CharacterGachaScreen);
    }

    public static async UniTask OpenMissionScreenAsync(
        this UIManager uiManager,
        CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(
            UIType.MissionScreen,
            cancellationToken);
    }

    public static void CloseMissionScreen(
        this UIManager uiManager)
    {
        uiManager.Close(UIType.MissionScreen);
    }

    public static UniTask OpenInventoryScreenAsync(
    this UIManager uiManager,
    CancellationToken cancellationToken)
    {
        return uiManager.OpenTestRootAsync(
            UIType.InventoryScreen,
            cancellationToken);
    }

    public static void CloseInventoryScreen(
        this UIManager uiManager)
    {
        uiManager.Close(UIType.InventoryScreen);
    }

    public static UniTask OpenAchievementScreenAsync(
    this UIManager uiManager,
    CancellationToken cancellationToken)
    {
        return uiManager.OpenTestRootAsync(
            UIType.AchievementScreen,
            cancellationToken);
    }

    public static void CloseAchievementScreen(
        this UIManager uiManager)
    {
        uiManager.Close(UIType.AchievementScreen);
    }
}