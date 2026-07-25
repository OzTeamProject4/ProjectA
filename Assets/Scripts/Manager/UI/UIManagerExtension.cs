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

        //if (baseUI is EquipmentListPopupView equipmentListPopupView)
        //{
        //    if (equipmentListPopupView._equipType == equipType && equipmentListPopupView._currentSelectedCharacterModel == characterModel)
        //    {
        //        return;
        //    }

        //    equipmentListPopupView.Init(equipType, characterModel);
        //}
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

    public static async UniTask OpenCraftItemInfoPopupAsync(this UIManager uiManager, string id, Vector3 position, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenOverlayRootAsync(UIType.CraftItemInfoPopup, cancellationToken);

        //if (baseUI is ItemPreviewPopupView itemPreviewPopupView)
        //{
        //    itemPreviewPopupView.Bind(id, position);
        //}
    }

    public static void CloseItemPreviewPopup(this UIManager uiManager)
    {
        uiManager.Close(UIType.CraftItemInfoPopup);
    }

    public static async UniTask OpenOverlayAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.Overlay, cancellationToken);
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
}
