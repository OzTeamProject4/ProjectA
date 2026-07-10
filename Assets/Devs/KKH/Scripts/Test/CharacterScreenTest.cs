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

    private GrowthDataProvider _dataProvider;
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
        }

        if (null != _detailView)
        {
            _detailView.OnUseItemButtonClicked -= HandleUseItemButtonClicked;
            _detailView.OnCloseButtonClicked -= HandleDetailClosed;
        }

        CloseItemSelectPopup();
    }

    private async UniTaskVoid InitializeAsync()
    {
        await GameManager.Instance.DataManager.PreloadDataAsync();

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
        _listView.OnItemSelected += HandleItemSelected;

        CharacterListViewModel listViewModel = new CharacterListViewModel(new List<CharacterModel>(_characterModels.Values));
        _listView.Bind(listViewModel, BuildDisplayInfos());
    }

    private void BuildTestCharacterModels()
    {
        // TODO - 07.08: 다음 스프린트(세이브/영속성 시스템)에서 교체 필요.
        // 지금은 "전체 캐릭터(GetAllCharacterIds) = 보유 캐릭터"로 임시 가정하고,
        // 전부 레벨 1 / _testStartStar 로 새로 생성한다.
        // 실제로는 세이브 데이터(또는 서버 응답)의 "보유 캐릭터 목록 + 저장된 성급/레벨/경험치/
        // 중복본/아이템 수량"을 기준으로 CharacterModel 을 생성(또는 복원)해야 한다.
        foreach (string dataId in _dataProvider.GetAllCharacterIds())
        {
            CharacterModel model = new CharacterModel(dataId, _testStartStar, _dataProvider);

            // 버튼 동작(아이템 사용/승급) 테스트를 위해 전체 경험치 아이템을 미리 지급.
            foreach (string expItemDataId in _dataProvider.GetAllExpItemIds())
            {
                model.AddExpItem(expItemDataId, 999);
            }

            model.AddDuplicate(999);

            _characterModels[dataId] = model;
        }
    }

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

    private void HandleItemSelected(string dataId)
    {
        if (!_characterModels.TryGetValue(dataId, out CharacterModel model))
        {
            Debug.LogWarning($"[CharacterScreenTest] CharacterModel 을 찾을 수 없습니다. dataId={dataId}");
            return;
        }

        CharacterDetailViewModel detailViewModel = new CharacterDetailViewModel(model, _dataProvider);
        _detailView.Bind(detailViewModel, dataId);
        _detailView.gameObject.SetActive(true);

        _listView.gameObject.SetActive(false);

        _currentDetailModel = model;
    }

    private void HandleDetailClosed()
    {
        _detailView.gameObject.SetActive(false);
        _listView.gameObject.SetActive(true);
    }

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

        ExpItemSelectPopupViewModel popupViewModel = new ExpItemSelectPopupViewModel(_currentDetailModel, _dataProvider);
        _itemSelectPopup.Bind(popupViewModel);
    }

    private void HandleItemPopupSelected(string dataId)
    {
        _detailView.ConfirmUseItem(dataId);
        _itemSelectPopup.RefreshSlots();
    }

    private void HandleItemPopupClosed()
    {
        CloseItemSelectPopup();
    }

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