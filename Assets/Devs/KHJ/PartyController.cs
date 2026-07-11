using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PartyController : MonoBehaviour
{
    private List<BattleCharacter> _partyCharacters;
    private int _currentCharacterIndex;
    private CinemachineCamera _cinemachineCamera;
    private PlayerInputActions _inputAction;
    private float _switchCoolTime = 3.0f;
    private float _lastSwitchTime;
    private List<CharacterAIController> _aiControllerList;
    private List<PlayerController> _playerControllerList;
    private List<CameraController> _cameraControllerList;

    private void Awake()
    {
        // TODO 희준 InputManager 준비되면 정리 필요
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
    private void Update()
    {
        if (_partyCharacters == null)
        {
            return;
        }

        if (_inputAction.Player.Switch.WasPressedThisFrame())
        {
            TrySwitchToNextCharacter();
        }
    }

    public void Initialize(List<BattleCharacter> characters, CinemachineCamera cinemachinCamera)
    {
        _partyCharacters = characters;
        _cinemachineCamera = cinemachinCamera;

        SetupControllers();
        SwitchCharacter(0);
    }

    private void SetupControllers()
    {
        _playerControllerList = new List<PlayerController>();
        _cameraControllerList = new List<CameraController>();
        _aiControllerList = new List<CharacterAIController>();

        for (int i = 0; i < _partyCharacters.Count; i++)
        {
            BattleCharacter character = _partyCharacters[i];
            PlayerController player = character.GetComponent<PlayerController>();
            CameraController camera = character.GetComponentInChildren<CameraController>();
            CharacterAIController ai = character.GetComponent<CharacterAIController>();

            if (player == null || camera == null || ai == null)
            {
                Debug.LogError($"{character.name}에 필요한 컨트롤러가 없음. 프리팹 확인");
                return;
            }

            player.enabled = false;
            camera.enabled = false;
            ai.enabled = false;
            ai.Initialize(_partyCharacters[0], character);

            _playerControllerList.Add(player);
            _cameraControllerList.Add(camera);
            _aiControllerList.Add(ai);
        }
    }

    public void SwitchCharacter(int index)
    {
        _currentCharacterIndex = index;
        BattleCharacter target = _partyCharacters[index];

        for (int i = 0; i < _playerControllerList.Count; i++)
        {
            bool isSelected = (i == index);
            _playerControllerList[i].enabled = isSelected;
            _cameraControllerList[i].enabled = isSelected;
            _aiControllerList[i].enabled = !isSelected;
            _aiControllerList[i].SetAIFollowTarget(target);
        }

        CameraController selectedCamera = _cameraControllerList[index];
        _cinemachineCamera.Target.TrackingTarget = selectedCamera.transform;
        selectedCamera.SetBehindCharacter(target.transform);
    }
   
    private void TrySwitchToNextCharacter()
    {
        if (Time.time - _lastSwitchTime < _switchCoolTime)
        {
            // TODO 희준 : 추후 UI에 표시 필요
            Debug.Log("아직 캐릭터 태그 기능 사용할수 없습니다");
            return;
        }

        int nextIndex = (_currentCharacterIndex + 1) % _partyCharacters.Count;
        SwitchCharacter(nextIndex);
        _lastSwitchTime = Time.time;
    }
}
