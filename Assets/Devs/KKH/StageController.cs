using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField] private StageMonsterParty _monsterPartyPrefab;
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
        await UniTask.Delay(1000, cancellationToken: destroyCancellationToken);

        if (_hasEntered)
        {
            Debug.LogWarning("[StageController] 이미 진입했습니다. 중복 실행을 건너뜁니다.");
            return;
        }

        _hasEntered = true;

        _screenStateModel = new ScreenStateModel(ScreenType.StageSelect);
        _progressModel = new StageProgressModel();

        _selectController = new StageSelectController(_monsterPartyPrefab, _selectMap, _progressModel);
        _selectController.SpawnParties();
    }
}