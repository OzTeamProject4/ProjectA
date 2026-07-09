using System.Collections.Generic;
using Unity.AppUI.UI;
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
            if (Time.time - _lastSwitchTime >= _switchCoolTime)
            {
                int nextIndex = (_currentCharacterIndex + 1) % _partyCharacters.Count;
                SwitchCharacter(nextIndex);
                _lastSwitchTime = Time.time;
            }

            else
            {
                // TODO 추후 UI에서 출력 필요. 임시 로그
                Debug.Log("아직 캐릭터 태그 기능을 사용할 수 없습니다");
            }
        }
    }

    public void Initialize(List<BattleCharacter> characters, CinemachineCamera cinemachinCamera)
    {
        _partyCharacters = characters;
        _cinemachineCamera = cinemachinCamera;

        GameObject controllObj = new GameObject("PlayerController");
        _playerController = controllObj.AddComponent<PlayerController>();

        GameObject pivotObj = new GameObject("CameraPivot");
        _cameraController = pivotObj.AddComponent<CameraController>();

        _cinemachineCamera.Target.TrackingTarget = pivotObj.transform;
        _playerController.SetCameraTransform(pivotObj.transform);

        SwitchCharacter(0);

        for (int i = 1; i < _partyCharacters.Count; i++)
        {
            BattleCharacter aiCharacter = _partyCharacters[i];
            CharacterAIController ai = aiCharacter.gameObject.AddComponent<CharacterAIController>();
            ai.Initialize(_partyCharacters[0], aiCharacter);
        }
    }

    public void SwitchCharacter(int index)
    {
        _currentCharacterIndex = index;
        BattleCharacter target = _partyCharacters[index];

        _playerController.SetControlTarget(target);
        _cameraController.SetTarget(target.transform);
    }
}
