using Cysharp.Threading.Tasks;
using System.Threading;

public static class UIManagerExtension
{
    //public static async UniTask OpenTestUIAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    //{
    //    BaseUI baseUI = await uiManager.OpenTestRootAsync(UIType.Test, cancellationToken);
    //}

    //public static void CloseTestUI(this UIManager uiManager)
    //{
    //    uiManager.Close(UIType.Test);
    //}

    public static async UniTask<CharacterListView> OpenCharacterListAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenContentRootAsync(UIType.CharacterList, cancellationToken);
        return GetView<CharacterListView>(baseUI, UIType.CharacterList);
    }

    public static void CloseCharacterList(this UIManager uiManager)
    {
        uiManager.Close(UIType.CharacterList);
    }

    public static async UniTask<CharacterDetailView> OpenCharacterDetailAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenContentRootAsync(UIType.CharacterDetail, cancellationToken);
        return GetView<CharacterDetailView>(baseUI, UIType.CharacterDetail);
    }

    public static void CloseCharacterDetail(this UIManager uiManager)
    {
        uiManager.Close(UIType.CharacterDetail);
    }

    public static async UniTask<ExpItemSelectPopupView> OpenExpItemSelectPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenOverlayRootAsync(UIType.ExpItemSelectPopup, cancellationToken);
        return GetView<ExpItemSelectPopupView>(baseUI, UIType.ExpItemSelectPopup);
    }

    public static void CloseExpItemSelectPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.ExpItemSelectPopup);
    }

    public static async UniTask<CraftPopupView> OpenCraftPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.CraftPopup, cancellationToken);
        return GetView<CraftPopupView>(baseUI, UIType.CraftPopup);
    }

    public static void CloseCraftPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.CraftPopup);
    }

    public static async UniTask<EquipmentListPopupView> OpenEquipmentListPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.EquipmentListPopup, cancellationToken);
        return GetView<EquipmentListPopupView>(baseUI, UIType.EquipmentListPopup);
    }

    public static void CloseEquipmentListPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.EquipmentListPopup);
    }

    public static async UniTask<EquipmentDetailPopupView> OpenEquipmentDetailPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenOverlayRootAsync(UIType.EquipmentDetailPopup, cancellationToken);
        return GetView<EquipmentDetailPopupView>(baseUI, UIType.EquipmentDetailPopup);
    }

    public static void CloseEquipmentDetailPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.EquipmentDetailPopup);
    }

    public static async UniTask<ItemPreviewPopupView> OpenItemPreviewPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenOverlayRootAsync(UIType.ItemPreviewPopup, cancellationToken);
        return GetView<ItemPreviewPopupView>(baseUI, UIType.ItemPreviewPopup);
    }

    public static void CloseItemPreviewPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.ItemPreviewPopup);
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

    public static async UniTask<PartySetupPopupView> OpenPartySetupPopupAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.PartySetupPopup, cancellationToken);
        return GetView<PartySetupPopupView>(baseUI, UIType.PartySetupPopup);
    }

    public static void ClosePartySetupPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.PartySetupPopup);
    }


    private static T GetView<T>(BaseUI baseUI, UIType uiType) where T : BaseUI
    {
        if (null == baseUI)
        {
            return null;
        }

        if (baseUI is not T view)
        {
            return null;
        }

        return view;
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
}
