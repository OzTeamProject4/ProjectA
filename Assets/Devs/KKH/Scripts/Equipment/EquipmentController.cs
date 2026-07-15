using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class EquipmentController
{
    private readonly Inventory _inventory;
    private readonly CraftingModel _craftingModel;

    private EquipmentListPopupView _listPopup;
    private CraftPopupView _craftPopup;
    private EquipmentDetailPopupView _detailPopup;

    private CharacterModel _characterModel;

    public EquipmentController(Inventory inventory, CraftingModel craftingModel)
    {
        if (null == inventory || null == craftingModel)
        {
            Debug.LogError("[EquipmentController] inventory 또는 craftingModel 이 null 입니다.");
        }

        _inventory = inventory;
        _craftingModel = craftingModel;
    }

    // 현재 대상 캐릭터 설정 (상세 화면 진입 시 호출)
    public void SetCharacter(CharacterModel characterModel)
    {
        _characterModel = characterModel;
    }

    // TODO: 시그니처 슬롯은 별도 팝업(강화 플로우 포함)으로 분리 예정
    public async UniTask OpenAsync(EquipType slotType, CancellationToken cancellationToken)
    {
        if (null == _characterModel)
        {
            Debug.LogWarning("[EquipmentController] 대상 캐릭터가 설정되지 않았습니다.");
            return;
        }

        CloseDetailPopup();

        await OpenListPopupAsync(slotType, cancellationToken);
        await OpenCraftPopupAsync(slotType, cancellationToken);
    }

    public void CloseAll()
    {
        CloseDetailPopup();
        CloseListPopup();
        CloseCraftPopup();
    }

    // 컨트롤러 파괴 시 이벤트 구독 해제
    public void Dispose()
    {
        UnsubscribeListPopup();
        UnsubscribeCraftPopup();
        UnsubscribeDetailPopup();

        _characterModel = null;
    }

    // ===== 보유 장비 목록 팝업 =====

    private async UniTask OpenListPopupAsync(EquipType slotType, CancellationToken cancellationToken)
    {
        _listPopup = await GameManager.Instance.UIManager.OpenEquipmentListPopupAsync(cancellationToken);

        if (null == _listPopup)
        {
            Debug.LogError("[EquipmentController] EquipmentListPopupView 를 열지 못했습니다.");
            return;
        }

        _listPopup.OnItemSelected -= HandleItemSelected;
        _listPopup.OnCloseButtonClicked -= HandleListPopupClosed;
        _listPopup.OnItemSelected += HandleItemSelected;
        _listPopup.OnCloseButtonClicked += HandleListPopupClosed;

        EquipmentListPopupViewModel viewModel = new EquipmentListPopupViewModel(_inventory, _characterModel, slotType);
        _listPopup.Bind(viewModel, slotType.ToString());
    }

    private void HandleListPopupClosed()
    {
        CloseDetailPopup();
        CloseListPopup();
    }

    private void CloseListPopup()
    {
        if (null == _listPopup)
        {
            return;
        }

        GameManager.Instance.UIManager.CloseEquipmentListPopup();
    }

    private void UnsubscribeListPopup()
    {
        if (null == _listPopup)
        {
            return;
        }

        _listPopup.OnItemSelected -= HandleItemSelected;
        _listPopup.OnCloseButtonClicked -= HandleListPopupClosed;
        _listPopup = null;
    }

    // ===== 장비 제작 팝업 =====

    private async UniTask OpenCraftPopupAsync(EquipType slotType, CancellationToken cancellationToken)
    {
        _craftPopup = await GameManager.Instance.UIManager.OpenCraftPopupAsync(cancellationToken);

        if (null == _craftPopup)
        {
            Debug.LogError("[EquipmentController] CraftPopupView 를 열지 못했습니다.");
            return;
        }

        _craftPopup.OnCrafted -= HandleCrafted;
        _craftPopup.OnCloseButtonClicked -= HandleCraftPopupClosed;
        _craftPopup.OnCrafted += HandleCrafted;
        _craftPopup.OnCloseButtonClicked += HandleCraftPopupClosed;

        CraftPopupViewModel viewModel = new CraftPopupViewModel(_craftingModel, _inventory, slotType, _characterModel);
        _craftPopup.Bind(viewModel);
    }

    private void HandleCrafted(string dataId)
    {
        Debug.Log($"제작 완료: {dataId}");
    }

    private void HandleCraftPopupClosed()
    {
        CloseCraftPopup();
    }

    private void CloseCraftPopup()
    {
        if (null == _craftPopup)
        {
            return;
        }

        GameManager.Instance.UIManager.CloseCraftPopup();
    }

    private void UnsubscribeCraftPopup()
    {
        if (null == _craftPopup)
        {
            return;
        }

        _craftPopup.OnCrafted -= HandleCrafted;
        _craftPopup.OnCloseButtonClicked -= HandleCraftPopupClosed;
        _craftPopup = null;
    }

    // ===== 장비 상세 팝업 =====

    private void HandleItemSelected(EquipmentListItemViewModel itemViewModel, RectTransform itemRect)
    {
        OpenDetailPopupAsync(itemViewModel, itemRect).Forget();
    }

    private async UniTaskVoid OpenDetailPopupAsync(EquipmentListItemViewModel itemViewModel, RectTransform itemRect)
    {
        if (null == _characterModel || null == itemViewModel)
        {
            return;
        }

        _detailPopup = await GameManager.Instance.UIManager.OpenEquipmentDetailPopupAsync();

        if (null == _detailPopup)
        {
            Debug.LogError("[EquipmentController] EquipmentDetailPopupView 를 열지 못했습니다.");
            return;
        }

        _detailPopup.OnEquipped -= HandleDetailPopupFinished;
        _detailPopup.OnUnequipped -= HandleDetailPopupFinished;
        _detailPopup.OnCloseButtonClicked -= HandleDetailPopupFinished;
        _detailPopup.OnEquipped += HandleDetailPopupFinished;
        _detailPopup.OnUnequipped += HandleDetailPopupFinished;
        _detailPopup.OnCloseButtonClicked += HandleDetailPopupFinished;

        EquipmentDetailPopupViewModel viewModel = new EquipmentDetailPopupViewModel(_characterModel, itemViewModel.Instance);
        _detailPopup.Bind(viewModel);

        MoveDetailPopupTo(itemRect);
    }

    // 탭한 아이템 바로 아래에 카드를 배치
    private void MoveDetailPopupTo(RectTransform itemRect)
    {
        if (null == itemRect || null == _detailPopup)
        {
            return;
        }

        Vector3[] itemCorners = new Vector3[4];
        itemRect.GetWorldCorners(itemCorners);

        Vector3 itemBottomCenter = (itemCorners[0] + itemCorners[3]) * 0.5f;

        _detailPopup.MoveCardTo(itemBottomCenter);
    }

    // TODO: 장착/해제 시 사운드·비주얼 피드백 추가
    private void HandleDetailPopupFinished()
    {
        CloseDetailPopup();

        if (null != _listPopup)
        {
            _listPopup.ClearSelection();
        }
    }

    private void CloseDetailPopup()
    {
        if (null == _detailPopup)
        {
            return;
        }

        GameManager.Instance.UIManager.CloseEquipmentDetailPopup();
    }

    private void UnsubscribeDetailPopup()
    {
        if (null == _detailPopup)
        {
            return;
        }

        _detailPopup.OnEquipped -= HandleDetailPopupFinished;
        _detailPopup.OnUnequipped -= HandleDetailPopupFinished;
        _detailPopup.OnCloseButtonClicked -= HandleDetailPopupFinished;
        _detailPopup = null;
    }
}