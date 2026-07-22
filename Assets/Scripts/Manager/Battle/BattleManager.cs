using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

// TODO 희준 : 팀 매니저 체계 편입 필요
// - BaseManager<BattleManager> 상속으로 변경
// - Awake의 초기화를 Initialize() 오버라이드로 이동
// - GameManager에 BattleManager 프로퍼티/Setup/Initialzie 추가 요청
public class BattleManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cinemachineCamera;

    private TempPartySpawner _partySpawner;
    private PartyController _partyController;

    private async void Start()
    {
        if (_cinemachineCamera == null)
        {
            Debug.LogError("카메라 참조 null 인스펙터 확인");
            return;
        }
        await EnterBattle();
    }
   
    private void OnDisable()
    {
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
    private async UniTask EnterBattle()
    {
        //TODO 희준 매니저 담당과 로드시점 협의 필요
        //await GameManager.Instance.DataManager.LoadDataAsync<CharacterData>("Data_TestCharacter");
        // GameManager InitializeTask 를 임시로 프로퍼티로 변경. 
        await GameManager.Instance.InitializeManagersAsync();
        await GameManager.Instance.DataManager.LoadRuntimeDataAsync();

        GameManager.Instance.InputManager.EnablePlayerActions();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //TODO 희준: 임시파티 ID, 추후 파티편성창에서 id받아오는 방식으로 교체
        List<string> tempPartyIds = new List<string> { "Character_004", "Character_005", "Character_003" };

        _partySpawner = new TempPartySpawner();
        List<BattleCharacter> characters = await _partySpawner.SpawnPartyById(tempPartyIds);

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

}
