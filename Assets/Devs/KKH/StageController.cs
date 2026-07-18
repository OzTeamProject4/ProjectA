using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField] private StageMonsterParty _monsterPartyPrefab;
    [SerializeField] private StagePlayerPartyRoot _playerPartyPrefab;
    [SerializeField] private StageSelectMap _selectMap; 

    private ScreenStateModel _screenStateModel;
    private StageProgressModel _progressModel;
    private StageSelectController _selectController;

    private bool _hasEntered;

    private void Start()
    {
        EnterAsync().Forget();
    }

    private void OnDestroy()
    {
        if (null != _selectController)
        {
            _selectController.Dispose();
        }
    }

    public async UniTask EnterAsync()
    {
        await GameManager.Instance.InitializeManagersAsync();

        if (_hasEntered)
        {
            Debug.LogWarning("[StageController] 이미 진입했습니다. 중복 실행을 건너뜁니다.");
            return;
        }

        _hasEntered = true;

        _screenStateModel = new ScreenStateModel(ScreenType.StageSelect);
        _progressModel = new StageProgressModel();

        IGameDataProvider dataProvider = new GameDataProvider();
        StagePlayerParty playerParty = SpawnPlayerParty();

        _selectController = new StageSelectController(_monsterPartyPrefab, _selectMap, _progressModel, dataProvider, _screenStateModel, playerParty);
        _selectController.SpawnParties();
    }

    private StagePlayerParty SpawnPlayerParty()
    {
        if (null == _playerPartyPrefab || null == _selectMap)
        {
            Debug.LogError("[StageController] _playerPartyPrefab 또는 _selectMap 이 null 입니다.");
            return null;
        }

        Transform spawnPoint = _selectMap.PlayerSpawnPoint;

        if (null == spawnPoint)
        {
            Debug.LogError("[StageController] 맵에 PlayerSpawnPoint 가 연결되지 않았습니다.");
            return null;
        }

        StagePlayerPartyRoot root = Instantiate(_playerPartyPrefab, spawnPoint.position, spawnPoint.rotation);

        return root.PlayerParty;
    }
}