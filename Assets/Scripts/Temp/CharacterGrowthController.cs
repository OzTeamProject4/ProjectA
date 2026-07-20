using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGrowthController : MonoBehaviour
{
    private bool _hasInitialized;

    private IGameDataProvider _dataProvider;
    private CraftingModel _craftingModel;
    private EquipmentController _equipmentFlow;
    private GrowthIconPreloader _iconPreloader;

    private GrowthModel _growthModel;

    private CharacterListView _listView;
    private CharacterDetailView _detailView;
    private ExpItemSelectPopupView _itemSelectPopup;

    private CharacterModel _currentDetailModel;

    private void OnDestroy()
    {
        UnsubscribeListView();
        UnsubscribeDetailView();
        UnsubscribeItemSelectPopup();
        UnsubscribeCurrentDetailModel();

        if (null != _equipmentFlow)
        {
            _equipmentFlow.Dispose();
        }

        if (null != _iconPreloader)
        {
            _iconPreloader.Release();
        }
    }

    // ===== 초기화 =====

    // 로비 진입점. 로비 담당자는 이 오브젝트 생성 후 EnterAsync() 호출만 하면 됨.
    public async UniTask EnterAsync()
    {
        if (_hasInitialized)
        {
            Debug.LogWarning("[CharacterGrowthController] 이미 초기화되었습니다. 중복 실행을 건너뜁니다.");
            return;
        }

        _hasInitialized = true;

        Inventory inventory = GameManager.Instance.Inventory;

        _dataProvider = new GameDataProvider();
        _craftingModel = new CraftingModel(inventory, _dataProvider);
        _equipmentFlow = new EquipmentController(inventory, _craftingModel);
        _iconPreloader = new GrowthIconPreloader(_dataProvider);

        _growthModel = GameManager.Instance.NetworkManager.ModelContainer.GetModel<GrowthModel>();

        if (null == _growthModel)
        {
            Debug.LogError("[CharacterGrowthController] GrowthModel 을 찾을 수 없습니다.");
            return;
        }

        _iconPreloader.PreloadAsync(destroyCancellationToken).Forget();

        await OpenCharacterListAsync();
    }

    private List<CharacterDisplayInfo> BuildDisplayInfos()
    {
        List<CharacterDisplayInfo> infos = new();

        foreach (string dataId in _growthModel.GetAllCharacters().Keys)
        {
            CharacterData data = _dataProvider.GetStat(dataId);

            infos.Add(new CharacterDisplayInfo
            {
                DataId = dataId,
                Name = null == data ? dataId : data.Name,
                Portrait = null // TODO: CharacterIconPath 로 초상화 로드
            });
        }

        return infos;
    }

    // ===== 캐릭터 목록 =====

    private async UniTask OpenCharacterListAsync()
    {
        _listView = await GameManager.Instance.UIManager.OpenCharacterListAsync(destroyCancellationToken);

        if (null == _listView)
        {
            Debug.LogError("[CharacterGrowthController] CharacterListView 를 열지 못했습니다.");
            return;
        }

        _listView.OnItemSelected -= HandleCharacterSelected;
        _listView.OnCloseButtonClicked -= HandleListClosed;
        _listView.OnItemSelected += HandleCharacterSelected;
        _listView.OnCloseButtonClicked += HandleListClosed;

        CharacterListViewModel viewModel = new CharacterListViewModel(new List<CharacterModel>(_growthModel.GetAllCharacters().Values));
        _listView.Bind(viewModel, BuildDisplayInfos());
    }

    private void HandleCharacterSelected(string dataId)
    {
        OpenCharacterDetailAsync(dataId).Forget();
    }

    // TODO: 로비 도입 시 "로비 진입" 시점으로 이 저장 호출 이관. 지금은 캐릭터 목록 닫기를 임시 대체 지점으로 사용.
    private void HandleListClosed()
    {
        GameManager.Instance.UIManager.CloseCharacterList();
        GameManager.Instance.NetworkManager.SaveGame();
    }

    private void UnsubscribeListView()
    {
        if (null == _listView)
        {
            return;
        }

        _listView.OnItemSelected -= HandleCharacterSelected;
        _listView.OnCloseButtonClicked -= HandleListClosed;
        _listView = null;
    }

    // ===== 캐릭터 상세 =====

    private async UniTaskVoid OpenCharacterDetailAsync(string dataId)
    {
        CharacterModel model = _growthModel.GetCharacter(dataId);

        if (null == model)
        {
            Debug.LogWarning($"[CharacterGrowthController] CharacterModel 을 찾을 수 없습니다. dataId={dataId}");
            return;
        }

        CloseAllPopups();

        UnsubscribeCurrentDetailModel();

        _currentDetailModel = model;
        _currentDetailModel.OnEquipmentChanged += HandleStatChanged;
        _currentDetailModel.OnStarChanged += HandleStatChanged;

        _equipmentFlow.SetCharacter(model);

        _detailView = await GameManager.Instance.UIManager.OpenCharacterDetailAsync(destroyCancellationToken);

        if (null == _detailView)
        {
            Debug.LogError("[CharacterGrowthController] CharacterDetailView 를 열지 못했습니다.");
            return;
        }

        _detailView.OnUseItemButtonClicked -= HandleUseItemButtonClicked;
        _detailView.OnCloseButtonClicked -= HandleDetailClosed;
        _detailView.OnEquipmentSlotClicked -= HandleEquipmentSlotClicked;
        _detailView.OnUseItemButtonClicked += HandleUseItemButtonClicked;
        _detailView.OnCloseButtonClicked += HandleDetailClosed;
        _detailView.OnEquipmentSlotClicked += HandleEquipmentSlotClicked;

        CharacterData data = _dataProvider.GetStat(dataId);
        string characterName = null == data ? dataId : data.Name;

        CharacterDetailViewModel viewModel = new CharacterDetailViewModel(model);
        _detailView.Bind(viewModel, characterName);

        GameManager.Instance.UIManager.CloseCharacterList();
    }

    private void HandleDetailClosed()
    {
        CloseAllPopups();

        GameManager.Instance.UIManager.CloseCharacterDetail();

        UnsubscribeCurrentDetailModel();

        OpenCharacterListAsync().Forget();
    }

    private void HandleStatChanged()
    {
        GameManager.Instance.NetworkManager.SaveGame();
    }

    private void UnsubscribeCurrentDetailModel()
    {
        if (null == _currentDetailModel)
        {
            return;
        }

        _currentDetailModel.OnEquipmentChanged -= HandleStatChanged;
        _currentDetailModel.OnStarChanged -= HandleStatChanged;
        _currentDetailModel = null;
    }

    private void UnsubscribeDetailView()
    {
        if (null == _detailView)
        {
            return;
        }

        _detailView.OnUseItemButtonClicked -= HandleUseItemButtonClicked;
        _detailView.OnCloseButtonClicked -= HandleDetailClosed;
        _detailView.OnEquipmentSlotClicked -= HandleEquipmentSlotClicked;
        _detailView = null;
    }

    // ===== 경험치 아이템 선택 팝업 =====

    private void HandleUseItemButtonClicked()
    {
        OpenItemSelectPopupAsync().Forget();
    }

    private async UniTaskVoid OpenItemSelectPopupAsync()
    {
        if (null == _currentDetailModel)
        {
            Debug.LogWarning("[CharacterGrowthController] 현재 선택된 캐릭터가 없습니다.");
            return;
        }

        _itemSelectPopup = await GameManager.Instance.UIManager.OpenExpItemSelectPopupAsync(destroyCancellationToken);

        if (null == _itemSelectPopup)
        {
            Debug.LogError("[CharacterGrowthController] ExpItemSelectPopupView 를 열지 못했습니다.");
            return;
        }

        _itemSelectPopup.OnItemSelected -= HandleExpItemSelected;
        _itemSelectPopup.OnCloseButtonClicked -= HandleItemSelectPopupClosed;
        _itemSelectPopup.OnItemSelected += HandleExpItemSelected;
        _itemSelectPopup.OnCloseButtonClicked += HandleItemSelectPopupClosed;

        ExpItemSelectPopupViewModel viewModel = new ExpItemSelectPopupViewModel(_currentDetailModel, GameManager.Instance.Inventory);
        _itemSelectPopup.Bind(viewModel);
    }

    private void HandleExpItemSelected(string dataId)
    {
        if (null == _detailView)
        {
            Debug.LogWarning("[Controller] _detailView 가 null 입니다.");
            return;
        }

        _detailView.ConfirmUseItem(dataId);
    }

    private void HandleItemSelectPopupClosed()
    {
        GameManager.Instance.UIManager.CloseExpItemSelectPopup();
    }

    private void UnsubscribeItemSelectPopup()
    {
        if (null == _itemSelectPopup)
        {
            return;
        }

        _itemSelectPopup.OnItemSelected -= HandleExpItemSelected;
        _itemSelectPopup.OnCloseButtonClicked -= HandleItemSelectPopupClosed;
        _itemSelectPopup = null;
    }

    // ===== 장비 (EquipmentController 에 위임) =====

    private void HandleEquipmentSlotClicked(EquipType slotType)
    {
        _equipmentFlow.OpenAsync(slotType, destroyCancellationToken).Forget();
    }

    // ===== 공통 =====

    private void CloseAllPopups()
    {
        if (null != _equipmentFlow)
        {
            _equipmentFlow.CloseAll();
        }

        if (null != _itemSelectPopup)
        {
            GameManager.Instance.UIManager.CloseExpItemSelectPopup();
        }
    }
}