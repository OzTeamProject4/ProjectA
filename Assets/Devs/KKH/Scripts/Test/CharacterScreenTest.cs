using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// TODO - 07.08: 다음 스프린트(세이브/영속성 시스템 도입) 시 BuildTestCharacterModels() 를
/// "보유 캐릭터 목록 + 저장된 성장 상태 로드" 방식으로 교체해야 한다. 자세한 내용은 해당 메서드 주석 참고.
/// </summary>
public class CharacterScreenTest : MonoBehaviour
{
    [SerializeField] private CharacterListView _listViewPrefab;
    [SerializeField] private CharacterDetailView _detailViewPrefab;
    [SerializeField] private ExpItemSelectPopupView _itemSelectPopupPrefab;
    [SerializeField] private Transform _contentParent;
    [SerializeField] private Transform _popupParent;

    [SerializeField] private int _testStartStar = 3;

    private bool _hasInitialized;

    private IGrowthDataProvider _dataProvider;
    private readonly Dictionary<string, CharacterModel> _characterModels = new();

    private CharacterListView _listView;
    private CharacterDetailView _detailView;
    private ExpItemSelectPopupView _itemSelectPopup;
    private CharacterModel _currentDetailModel;

    private void Start()
    {
        InitializeAsync().Forget();
    }

    private void OnDestroy()
    {
        if (null != _listView)
        {
            _listView.OnItemSelected -= HandleItemSelected;
            _listView.OnCloseButtonClicked -= HandleListClosed;
        }

        if (null != _detailView)
        {
            _detailView.OnUseItemButtonClicked -= HandleUseItemButtonClicked;
            _detailView.OnCloseButtonClicked -= HandleDetailClosed;
        }

        CloseItemSelectPopup();
    }

    // [로비 이관 대상] View 생성·배선 + Model/ViewModel 생성. 로비 도입 시 이 흐름 전체가 로비로 옮겨간다.
    private async UniTaskVoid InitializeAsync()
    {
        await UniTask.Delay(1000);

        if (_hasInitialized)
        {
            Debug.LogWarning("[CharacterScreenTest] 이미 초기화되었습니다. 중복 실행을 건너뜁니다.");
            return;
        }

        _hasInitialized = true;

        if (null == _listViewPrefab || null == _detailViewPrefab)
        {
            Debug.LogError("[CharacterScreenTest] View 프리팹이 연결되지 않았습니다.");
            return;
        }

        _dataProvider = new GrowthDataProvider();

        BuildTestCharacterModels();

        _detailView = Instantiate(_detailViewPrefab, _contentParent);
        _detailView.OnUseItemButtonClicked += HandleUseItemButtonClicked;
        _detailView.OnCloseButtonClicked += HandleDetailClosed;
        _detailView.gameObject.SetActive(false);

        _listView = Instantiate(_listViewPrefab, _contentParent);
        _listView.OnCloseButtonClicked += HandleListClosed;
        _listView.OnItemSelected += HandleItemSelected;

        CharacterListViewModel listViewModel = new CharacterListViewModel(new List<CharacterModel>(_characterModels.Values));
        _listView.Bind(listViewModel, BuildDisplayInfos());
    }

    // [로비 이관 대상] 임시 더미 데이터 생성. 세이브 시스템 + 로비 도입 시 함께 교체/이관.
    private void BuildTestCharacterModels()
    {
        Inventory inventory = GameManager.Instance.Inventory;

        foreach (string expItemDataId in _dataProvider.GetAllExpItemIds())
        {
            inventory.AddItem(expItemDataId, 999);
        }

        foreach (string dataId in _dataProvider.GetAllCharacterIds())
        {
            CharacterModel model = new CharacterModel(dataId, _testStartStar, _dataProvider, inventory);
            model.AddDuplicate(999);

            _characterModels[dataId] = model;
        }
    }

    // [로비 이관 대상] 표시용 정보(이름/초상화) 구성. 로비/컨트롤러로 함께 이관.
    private List<CharacterDisplayInfo> BuildDisplayInfos()
    {
        List<CharacterDisplayInfo> infos = new();

        foreach (string dataId in _characterModels.Keys)
        {
            infos.Add(new CharacterDisplayInfo
            {
                DataId = dataId,
                Name = dataId,
                Portrait = null
            });
        }

        return infos;
    }

    // [분리 이관 대상] ViewModel 생성·Bind(→로비) + 목록↔상세 화면 전환(→UIManager)이 섞여 있음.
    // 두 시스템 도입 시 이 메서드를 생성부와 전환부로 쪼개 각각 이관.
    private void HandleItemSelected(string dataId)
    {
        if (!_characterModels.TryGetValue(dataId, out CharacterModel model))
        {
            Debug.LogWarning($"[CharacterScreenTest] CharacterModel 을 찾을 수 없습니다. dataId={dataId}");
            return;
        }

        // (생성·배선 → 로비)
        CharacterDetailViewModel detailViewModel = new CharacterDetailViewModel(model);
        _detailView.Bind(detailViewModel, dataId);

        // (화면 전환 → UIManager)
        _detailView.gameObject.SetActive(true);
        _listView.gameObject.SetActive(false);

        _currentDetailModel = model;
    }

    // [UIManager 이관 대상] 상세→목록 화면 전환. UIManager 도입 시 SetActive 토글을 UIManager 호출로 대체.
    private void HandleDetailClosed()
    {
        _detailView.gameObject.SetActive(false);
        _listView.gameObject.SetActive(true);
    }

    private void HandleListClosed()
    {
        _listView.gameObject.SetActive(false);
    }

    // [UIManager 이관 대상] 아이템 선택 팝업 열기. UIManager 도입 시 팝업 생성/표시를 UIManager 로 위임.
    // (단, ExpItemSelectPopupViewModel 생성 자체는 생성·배선이라 로비/컨트롤러 성격도 일부 포함)
    private void HandleUseItemButtonClicked()
    {
        if (null != _itemSelectPopup)
        {
            Debug.LogWarning("[CharacterScreenTest] 이미 아이템 선택 팝업이 열려 있습니다.");
            return;
        }

        if (null == _currentDetailModel)
        {
            Debug.LogWarning("HandleUseItemButtonClicked: 현재 선택된 캐릭터가 없습니다.");
            return;
        }

        if (null == _itemSelectPopupPrefab)
        {
            Debug.LogError("ItemSelectPopupPrefab 이 연결되지 않았습니다.");
            return;
        }

        _itemSelectPopup = Instantiate(_itemSelectPopupPrefab, _popupParent);
        _itemSelectPopup.OnItemSelected += HandleItemPopupSelected;
        _itemSelectPopup.OnCloseButtonClicked += HandleItemPopupClosed;

        ExpItemSelectPopupViewModel popupViewModel = new ExpItemSelectPopupViewModel(_currentDetailModel, GameManager.Instance.Inventory);
        _itemSelectPopup.Bind(popupViewModel);
    }

    private void HandleItemPopupSelected(string dataId)
    {
        _detailView.ConfirmUseItem(dataId);
    }

    private void HandleItemPopupClosed()
    {
        CloseItemSelectPopup();
    }

    // [UIManager 이관 대상] 팝업 닫기/정리.
    private void CloseItemSelectPopup()
    {
        if (null == _itemSelectPopup)
        {
            return;
        }

        _itemSelectPopup.OnItemSelected -= HandleItemPopupSelected;
        _itemSelectPopup.OnCloseButtonClicked -= HandleItemPopupClosed;
        Destroy(_itemSelectPopup.gameObject);
        _itemSelectPopup = null;
    }
}