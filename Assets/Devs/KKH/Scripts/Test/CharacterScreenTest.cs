using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// TODO - 07.08: 세이브/영속성 시스템 도입 시 BuildTestCharacterModels() 를
/// "보유 캐릭터 목록 + 저장된 성장 상태 로드" 방식으로 교체. 자세한 내용은 해당 메서드 주석 참고.
public class CharacterScreenTest : MonoBehaviour
{
    [SerializeField] private CharacterListView _listViewPrefab;
    [SerializeField] private CharacterDetailView _detailViewPrefab;
    [SerializeField] private Transform _canvasParent;

    [SerializeField] private string _testUseItemId = "ExpBook_Small";
    [SerializeField] private int _testStartStar = 3;

    private bool _hasInitialized;

    private IGrowthDataProvider _dataProvider;
    private readonly Dictionary<string, CharacterModel> _characterModels = new();

    private CharacterListView _listView;
    private CharacterDetailView _detailView;

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
    }

    private async UniTaskVoid InitializeAsync()
    {
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

        _dataProvider = new MockGrowthDataProvider();
        await _dataProvider.InitializeAsync(this.GetCancellationTokenOnDestroy());

        BuildTestCharacterModels();

        _detailView = Instantiate(_detailViewPrefab, _canvasParent);
        _detailView.OnUseItemButtonClicked += HandleUseItemButtonClicked;
        _detailView.OnCloseButtonClicked += HandleDetailClosed;
        _detailView.gameObject.SetActive(false);

        _listView = Instantiate(_listViewPrefab, _canvasParent);
        _listView.OnItemSelected += HandleItemSelected;

        CharacterListViewModel listViewModel = new CharacterListViewModel(new List<CharacterModel>(_characterModels.Values));
        _listView.Bind(listViewModel, BuildDisplayInfos());
    }

    private void BuildTestCharacterModels()
    {
        // TODO - 07.08: 세이브/영속성 시스템 도입 시 교체 필요.
        // 지금은 "전체 캐릭터(GetAllCharacterIds) = 보유 캐릭터"로 임시 가정하고,
        // 전부 레벨 1 / _testStartStar 로 새로 생성.
        // 실제로는 세이브 데이터(또는 서버 응답)의 "보유 캐릭터 목록 + 저장된 성급/레벨/경험치/
        // 중복본/아이템 수량"을 기준으로 CharacterModel 을 생성해야 한다.
        foreach (string characterId in _dataProvider.GetAllCharacterIds())
        {
            CharacterModel model = new CharacterModel(characterId, _testStartStar, _dataProvider);

            // 버튼 동작(아이템 사용/승급) 테스트를 위해 미리 지급.
            model.AddExpItem(_testUseItemId, 999);
            model.AddDuplicate(999);

            _characterModels[characterId] = model;
        }
    }

    private List<CharacterDisplayInfo> BuildDisplayInfos()
    {
        List<CharacterDisplayInfo> infos = new();

        foreach (string characterId in _characterModels.Keys)
        {
            infos.Add(new CharacterDisplayInfo
            {
                CharacterId = characterId,
                Name = characterId,
                Portrait = null
            });
        }

        return infos;
    }

    private void HandleItemSelected(string characterId)
    {
        if (!_characterModels.TryGetValue(characterId, out CharacterModel model))
        {
            Debug.LogWarning($"CharacterModel 을 찾을 수 없습니다. CharacterId={characterId}");
            return;
        }

        CharacterDetailViewModel detailViewModel = new CharacterDetailViewModel(model, _dataProvider);
        _detailView.Bind(detailViewModel, characterId);
        _detailView.gameObject.SetActive(true);
    }

    private void HandleDetailClosed()
    {
        _detailView.gameObject.SetActive(false);
    }

    private void HandleUseItemButtonClicked()
    {
        _detailView.ConfirmUseItem(_testUseItemId);
    }
}