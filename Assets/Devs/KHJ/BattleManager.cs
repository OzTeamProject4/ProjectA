using Cysharp.Threading.Tasks;
using NUnit.Framework;
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
    [SerializeField] private PartySpawn _partySpawn;

    // private TempPartySpawner _partySpawner;
    private PartyController _partyController;
    //TODO 희준 : InputManger 준비되면 입력처리 이관
    private PlayerInputActions _inputAction;

    private void Awake()
    {
        _inputAction = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputAction.Player.Enable();
    }

    private void OnDisable()
    {
        _inputAction?.Player.Disable();
    }

    private async void Start()
    {
        if (_cinemachineCamera == null)
        {;
            Debug.LogError("카메라 참조 null 인스펙터 확인");
            return;
        }
        await GameManager.Instance.InitializeManagersAsync();
        await EnterBattle();
    }

    private void Update()
    {
        if (_inputAction.Player.Switch.WasPressedThisFrame())
        {
            _partyController.TrySwitchToNextCharacter();
        }
    }

    private async UniTask EnterBattle()
    {
        // await GameManager.Instance.DataManager.LoadDataAsync<CharacterData>("Data_TestCharacter");

        //TODO 희준: 임시파티 ID, 추후 파티편성창에서 id받아오는 방식으로 교체
        List<string> tempPartyIds = new List<string> { "Character_001", "Character_003", "Character_005" };

        //  _partySpawner = new TempPartySpawner();
        List<BattleCharacter> characters = await _partySpawn.SpawnPartyById(tempPartyIds);

        if (characters == null || characters.Count == 0)
        {
            Debug.LogError("파티 캐릭터가 null");
            return;
        }

        _partyController = new PartyController();
        _partyController.Initialize(characters, _cinemachineCamera);

    }


}
