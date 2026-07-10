using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PartyController : MonoBehaviour
{
    private List<BattleCharacter> _partyCharacters;
    private int _currentCharacterIndex;
    private PlayerController _playerController;
    private CameraController _cameraController;
    private CinemachineCamera _cinemachineCamera;
    private PlayerInputActions _inputAction;
    private float _switchCoolTime = 3.0f;
    private float _lastSwitchTime;
    private List<CharacterAIController> _aiControllerList;

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
        SetupAIControllers();
        SwitchCharacter(0);
    }

    public void SwitchCharacter(int index)
    {
        _currentCharacterIndex = index;
        BattleCharacter target = _partyCharacters[index];

        _playerController.SetControlTarget(target);
        _cameraController.SetTarget(target.transform);

        for (int i = 0; i < _aiControllerList.Count; i++)
        {
            if (i == index)
            {
                _aiControllerList[i].enabled = false;
            }

            else
            {
                _aiControllerList[i].enabled = true;
            }

            _aiControllerList[i].SetAIFollowTarget(target);
        }
    }

    private void SetupControllers()
    {
        GameObject controllObj = new GameObject("PlayerController");
        _playerController = controllObj.AddComponent<PlayerController>();

        GameObject pivotObj = new GameObject("CameraPivot");
        _cameraController = pivotObj.AddComponent<CameraController>();

        _cinemachineCamera.Target.TrackingTarget = pivotObj.transform;
        _playerController.SetCameraTransform(pivotObj.transform);
    }

    private void SetupAIControllers()
    {
        _aiControllerList = new List<CharacterAIController>();
        for (int i = 0; i < _partyCharacters.Count; i++)
        {
            BattleCharacter aiCharacter = _partyCharacters[i];
            CharacterAIController ai = aiCharacter.gameObject.AddComponent<CharacterAIController>();
            ai.enabled = false;
            ai.Initialize(_partyCharacters[0], aiCharacter);
            _aiControllerList.Add(ai);
        }
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
