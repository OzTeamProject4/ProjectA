using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIManagerExtension
{
    public static async UniTask OpenStudentManagementListAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenContentRootAsync(UIType.StudentManagementList, cancellationToken);
    }

    public static void CloseStudentManagementList(this UIManager uiManager)
    {
        uiManager.Close(UIType.StudentManagementList);
    }

    public static async UniTask OpenStudentManagementAsync(this UIManager uiManager, StudentModel studentModel, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenOverlayUIAsync();

        BaseUI baseUI = await uiManager.OpenContentRootAsync(UIType.StudentManagement, cancellationToken);

        if (baseUI is StudentManagementView studentManagementView)
        {
           // studentManagementView.Init(studentModel);
        }

        await uiManager.OpenOverlayUIAsync();
    }

    public static void CloseCharacterDetail(this UIManager uiManager)
    {
        uiManager.Close(UIType.StudentManagement);
    }

    //public static async UniTask OpenExpItemSelectPopupAsync(this UIManager uiManager, CharacterModel characterModel, CancellationToken cancellationToken = default)
    //{
    //    BaseUI baseUI = await uiManager.OpenOverlayRootAsync(UIType.ExpItemSelectPopup, cancellationToken);
    //    if (baseUI is ExpItemSelectPopupView expItemSelectPopupView)
    //    {
    //        expItemSelectPopupView.Init(characterModel);
    //    }
    //}

    public static void CloseExpItemSelectPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.ExpItemSelectPopup);
    }

    //public static async UniTask OpenCraftPopupAsync(this UIManager uiManager, EquipType equipType, CancellationToken cancellationToken = default)
    //{
    //    BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.CraftPopup, cancellationToken);

    //    if (baseUI is CraftPopupView craftPopupView)
    //    {
    //        if (craftPopupView._type == equipType)
    //        {
    //            return;
    //        }

    //        craftPopupView.Bind(equipType);
    //    }
    //}

    public static void CloseCraftPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.CraftPopup);
    }

    //public static async UniTask OpenEquipmentListPopupAsync(this UIManager uiManager, EquipType equipType, CharacterModel characterModel, CancellationToken cancellationToken = default)
    //{
    //    BaseUI baseUI = await uiManager.OpenPopupRootAsync(UIType.EquipmentListPopup, cancellationToken);

    //    if (baseUI is EquipmentListPopupView equipmentListPopupView)
    //    {
    //        if (equipmentListPopupView._equipType == equipType && equipmentListPopupView._currentSelectedCharacterModel == characterModel)
    //        {
    //            return;
    //        }

    //        equipmentListPopupView.Init(equipType, characterModel);
    //    }
    //}

    public static void CloseEquipmentListPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.EquipmentListPopup);
    }

    //public static async UniTask OpenEquipmentDetailPopupAsync(this UIManager uiManager, CharacterModel characterModel, ItemModel itemModel, Vector3 position, CancellationToken cancellationToken = default)
    //{
    //    BaseUI baseUI = await uiManager.OpenOverlayRootAsync(UIType.EquipmentDetailPopup, cancellationToken);

    //    if (baseUI is EquipmentDetailPopupView equipmentDetailPopupView)
    //    {
    //        equipmentDetailPopupView.Init(characterModel, itemModel, position);
    //    }
    //}

    public static void CloseEquipmentDetailPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.EquipmentDetailPopup);
    }

    //public static async UniTask OpenItemPreviewPopupAsync(this UIManager uiManager, string id, Vector3 position, CancellationToken cancellationToken = default)
    //{
    //    BaseUI baseUI = await uiManager.OpenOverlayRootAsync(UIType.ItemPreviewPopup, cancellationToken);

    //    if (baseUI is ItemPreviewPopupView itemPreviewPopupView)
    //    {
    //        itemPreviewPopupView.Bind(id, position);
    //    }
    //}

    public static void CloseItemPreviewPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.ItemPreviewPopup);
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
        await uiManager.OpenTestRootAsync(UIType.Overlay, cancellationToken);
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
