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

        if (GameManager.Instance != null)
        {
            GameManager.Instance.InputManager.OnUltimatePerformed -= HandleUltimate;
            GameManager.Instance.InputManager.OnBasicSkillPerformed -= HandleBasicSkill;
            GameManager.Instance.InputManager.OnNormalSkillPerformed -= HandleNormalSkill;
            GameManager.Instance.InputManager.OnSwitchIndexPerformed -= HandleSwitchIndex;
        }

        if (_partyController != null)
        {
            _partyController.Cleanup();
        }
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

    public async UniTask EnterBattle(Vector3 playerSpawnPosition, string stageId, CinemachineCamera battleCamera)
    {
        if (null == battleCamera)
        {
            Debug.LogError("[BattleManager] 전투 카메라가 null 입니다. BattleMap 프리팹의 BattleCamera 연결을 확인하세요.");
            return;
        }

        _cinemachineCamera = battleCamera;

        _stageId = stageId;
        _isBattleActive = true;

        GameManager.Instance.InputManager.EnablePlayerActions();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //TODO 희준: 임시파티 ID, 추후 파티편성창에서 id받아오는 방식으로 교체
        List<string> tempPartyIds = new List<string> { "Character_004", "Character_005", "Character_003" };
        _partySpawner = new TempPartySpawner();

        List<BattleCharacter> characters = await _partySpawner.SpawnPartyById(tempPartyIds, playerSpawnPosition);

        if (characters == null || characters.Count == 0)
        {
            Debug.LogError("파티 캐릭터가 null");
            return;
        }

        _partyController = new PartyController();
        _partyController.Initialize(characters, _cinemachineCamera);

        GameManager.Instance.InputManager.OnUltimatePerformed += HandleUltimate;
        GameManager.Instance.InputManager.OnBasicSkillPerformed += HandleBasicSkill;
        GameManager.Instance.InputManager.OnNormalSkillPerformed += HandleNormalSkill;
        GameManager.Instance.InputManager.OnSwitchIndexPerformed += HandleSwitchIndex;
    }

    public void EndBattle(bool isVictory)
    {
        _isBattleActive = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameManager.Instance.InputManager.DisablePlayerActions();

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

        BattlePauseChoice choice = await GameManager.Instance.UIManager.OpenBattlePauseAsync(destroyCancellationToken);

        _isPaused = false;
        Time.timeScale = 1f;

        if (choice == BattlePauseChoice.BackToStage)
        {
            _isBattleActive = false;
            OnReturnToSelectRequested?.Invoke();
            return;
        }

        GameManager.Instance.InputManager.EnablePlayerActions();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ===== 전투 결과창 =====

    private async UniTaskVoid ShowResultAsync(bool isVictory)
    {
        await GameManager.Instance.UIManager.OpenBattleResultAsync(isVictory, _stageId, destroyCancellationToken);

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
