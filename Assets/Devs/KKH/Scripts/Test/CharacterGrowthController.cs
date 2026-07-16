using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO - 세이브/영속성 시스템 도입 시 BuildTestCharacterModels() 를 "보유 캐릭터 + 저장된 성장 상태 로드"로 교체
/// TODO - UserManager 도입 시 _characterModels / _dataProvider / _craftingModel 소유를 그쪽으로 이관
/// TODO - 로비 도입 시 Start() 자동 시작 대신 외부에서 진입하도록 변경
/// </summary>
public class CharacterGrowthController : MonoBehaviour
{
    private bool _hasInitialized;

    private IGameDataProvider _dataProvider;
    private CraftingModel _craftingModel;
    private EquipmentController _equipmentFlow;
    private GrowthIconPreloader _iconPreloader;

    private readonly Dictionary<string, CharacterModel> _characterModels = new();

    private CharacterListView _listView;
    private CharacterDetailView _detailView;
    private ExpItemSelectPopupView _itemSelectPopup;

    private CharacterModel _currentDetailModel;

    private void OnDestroy()
    {
        UnsubscribeListView();
        UnsubscribeDetailView();
        UnsubscribeItemSelectPopup();

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

    // TODO: 로비 도입 시 이 흐름을 로비의 "캐릭터 화면 진입"으로 이관
    public async UniTaskVoid EnterAsync()
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

        BuildTestCharacterModels();

        _iconPreloader.PreloadAsync(destroyCancellationToken).Forget();

        await OpenCharacterListAsync();
    }

    // TODO: 세이브 시스템 도입 시 저장된 성장 상태 로드로 교체. 임시 재화/아이템 지급도 제거.
    private void BuildTestCharacterModels()
    {
        Inventory inventory = GameManager.Instance.Inventory;

        foreach (string expItemDataId in _dataProvider.GetAllExpItemIds())
        {
            inventory.AddItem(expItemDataId, 999);
        }

        inventory.AddItem("Item_Mat_T1", 999);
        inventory.AddItem("Item_Mat_T2", 999);
        inventory.AddItem("Item_Mat_T3", 999);
        inventory.AddItem("Item_Mat_Signature", 999);
        inventory.AddGold(999999);

        foreach (string dataId in _dataProvider.GetAllCharacterIds())
        {
            CharacterData data = _dataProvider.GetStat(dataId);

            if (null == data)
            {
                Debug.LogWarning($"[CharacterGrowthController] CharacterData 를 찾을 수 없습니다. dataId={dataId}");
                continue;
            }

            CharacterModel model = new CharacterModel(dataId, data.Star, _dataProvider, inventory);
            model.AddDuplicate(999);

            _characterModels[dataId] = model;
        }
    }

    private List<CharacterDisplayInfo> BuildDisplayInfos()
    {
        List<CharacterDisplayInfo> infos = new();

        foreach (string dataId in _characterModels.Keys)
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

        CharacterListViewModel viewModel = new CharacterListViewModel(new List<CharacterModel>(_characterModels.Values));
        _listView.Bind(viewModel, BuildDisplayInfos());
    }

    private void HandleCharacterSelected(string dataId)
    {
        OpenCharacterDetailAsync(dataId).Forget();
    }

    private void HandleListClosed()
    {
        GameManager.Instance.UIManager.CloseCharacterList();
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
        if (!_characterModels.TryGetValue(dataId, out CharacterModel model))
        {
            Debug.LogWarning($"[CharacterGrowthController] CharacterModel 을 찾을 수 없습니다. dataId={dataId}");
            return;
        }

        CloseAllPopups();

        _currentDetailModel = model;
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

        _currentDetailModel = null;

        OpenCharacterListAsync().Forget();
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