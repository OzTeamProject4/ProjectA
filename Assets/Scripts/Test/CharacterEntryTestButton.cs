using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterEntryTestButton : MonoBehaviour
{
    [SerializeField] private Button _enterButton;
    [SerializeField] private GameObject _testLobby;

    private CharacterGrowthController _controller;

    private void OnEnable()
    {
        if (null == _enterButton)
        {
            Debug.LogError("[CharacterEntryTestButton] _enterButton 이 할당되지 않았습니다.");
            return;
        }

        _enterButton.onClick.AddListener(HandleEnterButtonClicked);
    }

    private void OnDisable()
    {
        if (null == _enterButton)
        {
            return;
        }

        _enterButton.onClick.RemoveListener(HandleEnterButtonClicked);
    }

    private void HandleEnterButtonClicked()
    {
        if (null != _controller)
        {
            Debug.Log("[CharacterEntryTestButton] 이미 진입한 상태입니다.");
            return;
        }

        GameObject controllerObject = new GameObject("CharacterGrowthController");
        _controller = controllerObject.AddComponent<CharacterGrowthController>();

        _controller.EnterAsync().Forget();

        if (null != _testLobby)
        {
            _testLobby.SetActive(false);
        }
    }
}