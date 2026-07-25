using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO 희준 : 팀 매니저 체계 편입 필요
// - BaseManager<BattleManager> 상속으로 변경
// - Awake의 초기화를 Initialize() 오버라이드로 이동
// - GameManager에 BattleManager 프로퍼티/Setup/Initialzie 추가 요청
public class BattleManager : BaseManager<BattleManager>
{
    private CinemachineCamera _cinemachineCamera;
    private TempPartySpawner _partySpawner;
    private PartyController _partyController;

    public event Action<bool> OnBattleEnded;
    public event Action OnReturnToSelectRequested;

    private string _stageId;
    private bool _isBattleActive;
    private bool _isPaused;
    private bool _isInputSubscribed;

    private GameObject _enemyRoot;
    private GameObject _enemySkillRoot;

    public override UniTask InitializeAsync()
    {
        if (_enemyRoot == null)
        {
            _enemyRoot = new GameObject("EnemyRoot");
        }

        if (_enemySkillRoot == null)
        {
            _enemySkillRoot = new GameObject("EnemySkillRoot");
        }

        return UniTask.CompletedTask;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;

        UnsubscribeInputActions();
        CleanupBattleObjects();
    }

    private void CleanupBattleObjects()
    {
        DespawnChildren(_enemyRoot);
        DespawnChildren(_enemySkillRoot);

        CleanupPartyController();
    }

    private void DespawnChildren(GameObject rootObject)
    {
        if (rootObject == null || GameManager.Instance == null)
        {
            return;
        }

        Transform rootTransform = rootObject.transform;

        // Despawn 하면 부모가 풀 루트로 바뀌므로 역순으로 순회
        for (int i = rootTransform.childCount - 1; i >= 0; i--)
        {
            GameObject child = rootTransform.GetChild(i).gameObject;

            GameManager.Instance.ObjectManager.Despawn(child);
        }
    }

    private void SubscribeInputActions()
    {
        if (_isInputSubscribed)
        {
            return;
        }

        if (GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.InputManager.OnUltimatePerformed += HandleUltimate;
        GameManager.Instance.InputManager.OnBasicSkillPerformed += HandleBasicSkill;
        GameManager.Instance.InputManager.OnNormalSkillPerformed += HandleNormalSkill;
        GameManager.Instance.InputManager.OnSwitchIndexPerformed += HandleSwitchIndex;

        _isInputSubscribed = true;
    }

    private void UnsubscribeInputActions()
    {
        if (!_isInputSubscribed)
        {
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.InputManager.OnUltimatePerformed -= HandleUltimate;
            GameManager.Instance.InputManager.OnBasicSkillPerformed -= HandleBasicSkill;
            GameManager.Instance.InputManager.OnNormalSkillPerformed -= HandleNormalSkill;
            GameManager.Instance.InputManager.OnSwitchIndexPerformed -= HandleSwitchIndex;
        }

        _isInputSubscribed = false;
    }

    private void CleanupPartyController()
    {
        if (_partyController == null)
        {
            return;
        }

        _partyController.Cleanup();
        _partyController = null;
    }

    private void Update()
    {
        if (!_isBattleActive || _isPaused)
        {
            return;
        }

        if (null == Keyboard.current || !Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        ShowPauseAsync().Forget();
    }

    public async UniTask EnterBattle(Vector3 playerSpawnPosition, string stageId, CinemachineCamera battleCamera, IReadOnlyList<string> partyCharacterIds)
    {
        if (null == battleCamera)
        {
            Debug.LogError("[BattleManager] 전투 카메라가 null 입니다. BattleMap 프리팹의 BattleCamera 연결을 확인하세요.");
            return;
        }

        if (null == partyCharacterIds || partyCharacterIds.Count == 0)
        {
            Debug.LogError("[BattleManager] 편성된 캐릭터가 없습니다.");
            return;
        }

        _cinemachineCamera = battleCamera;

        _stageId = stageId;
        _isBattleActive = true;

        GameManager.Instance.InputManager.EnablePlayerActions();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _partySpawner = new TempPartySpawner();

        List<BattleCharacter> characters = await _partySpawner.SpawnPartyById(partyCharacterIds, playerSpawnPosition);

        if (characters == null || characters.Count == 0)
        {
            Debug.LogError("파티 캐릭터가 null");

            _isBattleActive = false;
            return;
        }

        CleanupPartyController();

        _partyController = new PartyController();
        _partyController.Initialize(characters, _cinemachineCamera);

        SubscribeInputActions();
    }

    public void EndBattle(bool isVictory)
    {
        _isBattleActive = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameManager.Instance.InputManager.DisablePlayerActions();

        UnsubscribeInputActions();

        OnBattleEnded?.Invoke(isVictory);

        ShowResultAsync(isVictory).Forget();
    }

    // ===== 일시정지 메뉴 (ESC 로 열림) =====

    private async UniTaskVoid ShowPauseAsync()
    {
        _isPaused = true;

        Time.timeScale = 0f;
        GameManager.Instance.InputManager.DisablePlayerActions();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        BattlePauseChoice choice = await WaitForPauseChoiceAsync();

        _isPaused = false;
        Time.timeScale = 1f;

        if (choice == BattlePauseChoice.BackToStage)
        {
            _isBattleActive = false;

            UnsubscribeInputActions();

            CleanupBattleObjects();

            OnReturnToSelectRequested?.Invoke();
            return;
        }

        GameManager.Instance.InputManager.EnablePlayerActions();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private async UniTask<BattlePauseChoice> WaitForPauseChoiceAsync()
    {
        BattlePausePopupView view = await GameManager.Instance.UIManager.OpenBattlePauseAsync(destroyCancellationToken);

        if (null == view)
        {
            Debug.LogError("[BattleManager] 일시정지 팝업을 열지 못했습니다. 전투를 재개합니다.");
            return BattlePauseChoice.Resume;
        }

        BattlePauseChoice choice = await view.WaitForChoiceAsync();

        GameManager.Instance.UIManager.CloseBattlePause();

        return choice;
    }

    // ===== 전투 결과창 =====

    private async UniTaskVoid ShowResultAsync(bool isVictory)
    {
        BattleResultPopupView view = await GameManager.Instance.UIManager.OpenBattleResultAsync(isVictory, _stageId, destroyCancellationToken);

        if (null != view)
        {
            await view.WaitForReturnAsync(isVictory, _stageId);

            GameManager.Instance.UIManager.CloseBattleResult();
        }
        else
        {
            Debug.LogError("[BattleManager] 전투 결과 팝업을 열지 못했습니다.");
        }

        CleanupBattleObjects();

        OnReturnToSelectRequested?.Invoke();
    }

    private void HandleUltimate()
    {
        if (_partyController == null)
        {
            return;
        }

        _partyController.UseCurrentCharacterUlt();
    }

    private void HandleBasicSkill()
    {
        if (_partyController == null)
        {
            return;
        }

        _partyController.UseCurrentCharacterBasicSkill();
    }

    private void HandleNormalSkill()
    {
        if (_partyController == null)
        {
            return;
        }

        _partyController.UseCurrentCharacterNormalSkill();
    }

    private void HandleSwitchIndex(int index)
    {
        if (_partyController == null)
        {
            return;
        }

        _partyController.TrySwitchToCharacter(index);
    }


    public async UniTask SpawnEnemyAsync(string enemyDataId, Transform enemySpawnTransform)
    {
        EnemyViewModel vm = new EnemyViewModel();

        if (GameManager.Instance.DataManager.TryGetData<EnemyData>(enemyDataId, out EnemyData enemyData))
        {
            if (enemyData == null)
            {
                Debug.LogError("적 데이터를 로드하지 못했습니다.");
                return;
            }

            GameObject prefab = await GameManager.Instance.ObjectManager.SpawnAsync(enemyData.PrefabAddress, _enemyRoot.transform, enemySpawnTransform);

            if (prefab == null)
            {
                Debug.LogError("적 프리팹을 로드하지 못했습니다.");
                return;
            }

            EnemyController enemyController = prefab.GetComponent<EnemyController>();

            if (enemyController == null)
            {
                Debug.LogError($"[BattleManager] 생성된 적 프리팹에 {nameof(EnemyController)} 가 없습니다. key={enemyData.PrefabAddress}");
                return;
            }

            enemyController.Bind(enemyData, vm);

            if (prefab.TryGetComponent<EnemyView>(out var enemyView))
            {
                enemyView.BindEnemyViewModel(vm);
            }
            else
            {
                Debug.LogError("생성된 에셋에 EnemyView 컴포넌트가 없습니다.");
            }

            if (GameManager.Instance.DataManager.TryGetData<EnemySkillData>(enemyData.SkillDataId, out EnemySkillData enemySkillData))
            {
                await GameManager.Instance.ObjectManager.PrewarmAsync(
           enemySkillData.PrefabAddress,
           10,
           destroyCancellationToken
           );
            }

        }
    }
    public async UniTask SpawnEnemySkillAsync(string skillDataId, Transform spawnTransform, Transform rotationTransform, EnemyController enemyController)
    {
        if (spawnTransform == null || rotationTransform == null)
        {
            Debug.LogError($"[SpawnEnemySkillAsync] spawnTransform 또는 rotationTransform이 null입니다. (SkillId: {skillDataId})");
            return;
        }

        EnemySkillViewModel vm = new EnemySkillViewModel();

        if (GameManager.Instance.DataManager.TryGetData<EnemySkillData>(skillDataId, out EnemySkillData skillData))
        {
            if (skillData == null)
            {
                Debug.LogError("적 데이터를 로드하지 못했습니다.");
                return;
            }

            GameObject prefab = await GameManager.Instance.ObjectManager.SpawnAsync(skillData.PrefabAddress, _enemySkillRoot.transform, spawnTransform);
            if (prefab == null)
            {
                Debug.LogError("스킬 프리팹을 로드하지 못했습니다.");
                return;
            }
            prefab.transform.rotation = rotationTransform.rotation;

            var enemySkillController = prefab.GetComponent<EnemySkillController>();
            enemySkillController.Bind(skillData, vm, enemyController);

        }
    }
}
