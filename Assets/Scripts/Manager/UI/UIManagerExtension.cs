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
