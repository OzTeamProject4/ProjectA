using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _height = 40f;

    private Transform _target;

    private void Awake()
    {
        UnityUtil.ValidateReference(_camera, nameof(MinimapCamera), nameof(_camera));
    }

    private void OnEnable()
    {
        if (GameManager.Instance == null || GameManager.Instance.BattleManager == null)
        {
            Debug.LogWarning($"[{nameof(MinimapCamera)}:{nameof(OnEnable)}] BattleManager를 찾을 수 없어 캐릭터 전환을 구독하지 못했습니다.");
            return;
        }

        GameManager.Instance.BattleManager.OnPartyCharacterChanged += HandlePartyCharacterChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null || GameManager.Instance.BattleManager == null)
        {
            return;
        }

        GameManager.Instance.BattleManager.OnPartyCharacterChanged -= HandlePartyCharacterChanged;
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 targetPosition = _target.position;

        transform.position = new Vector3(targetPosition.x, targetPosition.y + _height, targetPosition.z);
    }

    private void HandlePartyCharacterChanged(BattleCharacter battleCharacter)
    {
        if (battleCharacter == null)
        {
            _target = null;
            return;
        }

        _target = battleCharacter.transform;
    }
}
