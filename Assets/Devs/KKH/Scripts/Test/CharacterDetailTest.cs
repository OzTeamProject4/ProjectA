using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

// CharacterGrowth 시스템 동작 확인용 임시 테스트 스크립트.
public class CharacterDetailTest : MonoBehaviour
{
    [SerializeField] private CharacterDetailView _characterViewPrefab;
    [SerializeField] private Transform _canvasParent;

    [SerializeField] private string _testCharacterId = "Character_001";
    [SerializeField] private int _testStartStar = 3;
    [SerializeField] private string _testCharacterName = "Test Character";
    [SerializeField] private string _testUseItemId = "ExpBook_Small";

    private CharacterDetailView _view;

    private void Start()
    {
        InitializeAsync().Forget();
    }

    private void OnDestroy()
    {
        if (null != _view)
        {
            _view.OnUseItemButtonClicked -= HandleUseItemButtonClicked;
        }
    }

    private async UniTaskVoid InitializeAsync()
    {
        if (null == _characterViewPrefab)
        {
            Debug.LogError("CharacterView 프리팹이 연결되지 않았습니다.");
            return;
        }

        if (null == _canvasParent)
        {
            Debug.LogWarning("CanvasParent 가 비어 있습니다.");
        }

        CharacterDetailView view = Instantiate(_characterViewPrefab, _canvasParent);

        _view = Instantiate(_characterViewPrefab, _canvasParent);

        IGrowthDataProvider dataProvider = new MockGrowthDataProvider();
        await dataProvider.InitializeAsync(this.GetCancellationTokenOnDestroy());

        CharacterModel model = new CharacterModel(_testCharacterId, _testStartStar, dataProvider);

        // 버튼 동작(아이템 사용/승급) 테스트를 위해 미리 지급.
        model.AddExpItem(_testUseItemId, 999);
        model.AddDuplicate(999);

        CharacterDetailViewModel viewModel = new CharacterDetailViewModel(model, dataProvider);
        _view.Bind(viewModel, _testCharacterName);

        // 아이템 선택 팝업이 아직 없어, 팝업이 할 일(아이템 선택 확정)을 테스트 코드가 대신 수행.
        _view.OnUseItemButtonClicked += HandleUseItemButtonClicked;
    }

    private void HandleUseItemButtonClicked()
    {
        _view.ConfirmUseItem(_testUseItemId);
    }
}