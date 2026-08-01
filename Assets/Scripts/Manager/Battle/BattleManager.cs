using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class BattleManager : BaseManager<BattleManager>
{
    private const float DefaultBattleTime = 180f;

    private CinemachineCamera _cinemachineCamera;
    private TempPartySpawner _partySpawner;
    private PartyController _partyController;
    private BattleHUDPresenter _hudPresenter;
    private BattleTimer _battleTimer;
    private List<string> _loadedPortraitKeys = new List<string>();

    public event Action<bool> OnBattleEnded;
    public event Action OnReturnToSelectRequested;
    public event Action OnRetryRequested;

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

    private void Update()
    {
        if (!_isBattleActive || _isPaused)
        {
            return;
        }
// In Unity Test
#if UNITY_EDITOR
        if (Keyboard.current != null)
        {
            // N: 강제 승리
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                EndBattle(true);
                return;
            }

            // M: 강제 패배
            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                EndBattle(false);
                return;
            }
        }
#endif

        CheckPartyDefeat();

        if (!_isBattleActive)
        {
            return;
        }

        if (_battleTimer != null)
        {
            _battleTimer.Tick(Time.deltaTime);
        }

        if (_hudPresenter != null)
        {
            _hudPresenter.Tick();
        }


        if (null == Keyboard.current || !Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        ShowPauseAsync().Forget();
    }

    private void CheckPartyDefeat()
    {
        IReadOnlyList<BattleCharacter> party = _partyController?.PartyCharacters;

        if (party == null || party.Count == 0)
        {
            return;
        }

        for (int i = 0; i < party.Count; i++)
        {
            BattleCharacter character = party[i];

            if (character != null && character.CurHp > 0f)
            {
                return;
            }
        }

        EndBattle(false);
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UnsubscribeInputActions();
        CleanupBattleObjects();
    }

    private void CleanupBattleObjects()
    {
        DespawnChildren(_enemyRoot);
        DespawnChildren(_enemySkillRoot);

        if (GameManager.Instance != null)
        {
            foreach (string key in _loadedPortraitKeys)
            {
                GameManager.Instance.ResourceManager.ReleaseAsset(key);
            }
        }
        _loadedPortraitKeys.Clear();
        CleanupPartyController();
    }

    private void StopEnemies()
    {
        StopEnemyAgents();

        DespawnChildren(_enemySkillRoot);
    }

    private void StopEnemyAgents()
    {
        if (_enemyRoot == null)
        {
            return;
        }

        Transform rootTransform = _enemyRoot.transform;

        for (int i = 0; i < rootTransform.childCount; i++)
        {
            GameObject child = rootTransform.GetChild(i).gameObject;

            if (child.TryGetComponent(out Unity.Behavior.BehaviorGraphAgent behaviorGraphAgent))
            {
                behaviorGraphAgent.enabled = false;
            }

            if (child.TryGetComponent(out NavMeshAgent navMeshAgent))
            {
                StopNavMeshAgent(navMeshAgent);
            }

            if (child.TryGetComponent(out EnemyController enemyController))
            {
                enemyController.ChangeState(EnemyBattleState.Idle);
            }
        }
    }

    private void StopNavMeshAgent(NavMeshAgent navMeshAgent)
    {
        if (!navMeshAgent.isActiveAndEnabled || !navMeshAgent.isOnNavMesh)
        {
            return;
        }

        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;
    }

    private void ResumeEnemyAgent(GameObject enemyObject)
    {
        if (enemyObject.TryGetComponent(out NavMeshAgent navMeshAgent))
        {
            if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.isStopped = false;
            }
        }

        if (enemyObject.TryGetComponent(out Unity.Behavior.BehaviorGraphAgent behaviorGraphAgent))
        {
            behaviorGraphAgent.enabled = true;
        }
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
        if (_hudPresenter != null)
        {
            _hudPresenter.Cleanup();
            _hudPresenter = null;
        }

        if (_battleTimer != null)
        {
            _battleTimer.OnTimeOver -= HandleTimeOver;
            _battleTimer = null;
        }

        if (_partyController == null)
        {
            return;
        }

        _partyController.Cleanup();
        _partyController = null;
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

        bool hasStageData = GameManager.Instance.DataManager.TryGetData(stageId, out StageData stageData);

        if (!hasStageData)
        {
            Debug.LogError($"{_stageId}StageData를 찾을수 없음");
        }

        _battleTimer = new BattleTimer(GetBattleTime(hasStageData ? stageData : null));
        _battleTimer.OnTimeOver += HandleTimeOver;

        BattleHUDView hudView = await GameManager.Instance.UIManager.OpenBattleHUDAsync(destroyCancellationToken);

        if (hasStageData)
        {
            hudView.SetStage(stageData.StageName);
        }

        _hudPresenter = new BattleHUDPresenter();
        _hudPresenter.Initialize(hudView, _partyController, _battleTimer);
        await LoadPartyPortraitsAsync(characters, hudView);
        _partyController.Initialize(characters, _cinemachineCamera);
        _battleTimer.StartTimer();
        SubscribeInputActions();
    }

    private float GetBattleTime(StageData stageData)
    {
        if (null == stageData || stageData.TimeLimit <= 0f)
        {
            return DefaultBattleTime;
        }

        return stageData.TimeLimit;
    }

    public void EndBattle(bool isVictory)
    {

        if (!_isBattleActive)
        {
            return;
        }

        _isBattleActive = false;

        StopEnemies();

        GameManager.Instance.UIManager.CloseBattleHUD();
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

            GameManager.Instance.UIManager.CloseBattleHUD();

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
        if (isVictory)
        {
            GrantStageRewards();
        }

        BattleResultPopupView view = await GameManager.Instance.UIManager.OpenBattleResultAsync(isVictory, _stageId, destroyCancellationToken);

        BattleResultChoice choice = BattleResultChoice.Return;

        if (null != view)
        {
            choice = await view.WaitForChoiceAsync(isVictory, _stageId);

            GameManager.Instance.UIManager.CloseBattleResult();
        }
        else
        {
            Debug.LogError("[BattleManager] 전투 결과 팝업을 열지 못했습니다.");
        }

        CleanupBattleObjects();

        if (choice == BattleResultChoice.Retry)
        {
            OnRetryRequested?.Invoke();
            return;
        }

        OnReturnToSelectRequested?.Invoke();
    }

    private void GrantStageRewards()
    {
        if (!GameManager.Instance.DataManager.TryGetData(_stageId, out StageData stageData))
        {
            return;
        }

        if (!stageData.TryGetRewards(out (string ItemId, int Count)[] rewards))
        {
            return;
        }

        foreach ((string ItemId, int Count) reward in rewards)
        {
            NetworkManagerTemp.Instance.InventoryModel.GrantMaterial(reward.ItemId, reward.Count);
        }
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

    public async UniTask<EnemyViewModel> SpawnEnemyAsync(string enemyDataId, Transform enemySpawnTransform, bool isBoss = false)
    {
        if (!_isBattleActive)
        {
            return null;
        }

        EnemyViewModel vm = new EnemyViewModel();

        if (GameManager.Instance.DataManager.TryGetData<EnemyData>(enemyDataId, out EnemyData enemyData))
        {
            if (enemyData == null)
            {
                Debug.LogError("적 데이터를 로드하지 못했습니다.");
                return null;
            }

            GameObject prefab = await GameManager.Instance.ObjectManager.SpawnAsync(enemyData.PrefabAddress, _enemyRoot.transform, enemySpawnTransform);

            if (prefab == null)
            {
                Debug.LogError("적 프리팹을 로드하지 못했습니다.");
                return null;
            }

            ResumeEnemyAgent(prefab);

            EnemyController enemyController = prefab.GetComponent<EnemyController>();

            if (enemyController == null)
            {
                Debug.LogError($"[BattleManager] 생성된 적 프리팹에 {nameof(EnemyController)} 가 없습니다. key={enemyData.PrefabAddress}");
                return null;
            }

            enemyController.Bind(enemyData, vm);

            if (prefab.TryGetComponent<EnemyView>(out var enemyView))
            {
                enemyView.BindEnemyViewModel(vm);
                enemyView.SetHead(prefab.transform);
                var enemyHUD = await GameManager.Instance.UIManager.OpenEnemyHudUI();
                await enemyHUD.AddEnemyHudSlot(vm,enemyView.HeadAnchor);

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

            if (isBoss && _hudPresenter != null)
            {
                _hudPresenter.SetBossEnemy(vm);
            }

            return vm;
        }

        return null;
    }
    public async UniTask SpawnEnemySkillAsync(string skillDataId, Transform spawnTransform, Transform rotationTransform, EnemyController enemyController)
    {
        if (!_isBattleActive)
        {
            return;
        }

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

    private void HandleTimeOver()
    {
        EndBattle(false);
    }
    private async UniTask LoadPartyPortraitsAsync(List<BattleCharacter> characters, BattleHUDView hudView)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            BattleCharacter character = characters[i];
            if (character == null)
            {
                continue;
            }

            string iconPath = character.CharacterIconPath;
            if (string.IsNullOrEmpty(iconPath) == false)
            {
                Sprite portrait = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(iconPath);
                if (portrait != null)
                {
                    character.SetPortraitSprite(portrait);
                    _loadedPortraitKeys.Add(iconPath);
                }
                else
                {
                    Debug.LogError($"초상화 로드 실패 {iconPath}");
                }
            }

            string elementIconKey = GetElementIconKey(character.ElementType);
            if (string.IsNullOrEmpty(elementIconKey) == false)
            {
                Sprite elementIcon = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(elementIconKey);
                if (elementIcon != null)
                {
                    character.SetElementIcon(elementIcon);
                    _loadedPortraitKeys.Add(elementIconKey);
                }
                else
                {
                    Debug.LogError($"속성 아이콘 로드 실패 {elementIconKey}");
                }
            }
        }
    }
    private string GetElementIconKey(ElementType elementType)
    {
        // UI_Icon 시트 sub-sprite 인덱스 매핑 (11=불, 26=물, 20=풀, 12=무)
        // 스프라이트 시트 재slice 시 인덱스가 바뀔 수 있으니 주의
        switch (elementType)
        {
            case ElementType.Fire:
                return "UI/UI_Icon[UI_Icon_11]";
            case ElementType.Water:
                return "UI/UI_Icon[UI_Icon_26]";
            case ElementType.Grass:
                return "UI/UI_Icon[UI_Icon_20]";
            case ElementType.Normal:
                return "UI/UI_Icon[UI_Icon_12]";
            default:
                return string.Empty;
        }
    }
}
