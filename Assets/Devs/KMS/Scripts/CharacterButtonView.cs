using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CharacterButtonView : MonoBehaviour
{
    private Button _button;
    private CharacterGrowthController _controller;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null.");
            return;
        }

        if (_controller != null)
        {
            Debug.Log("이미 캐릭터 화면에 진입한 상태입니다.");
            return;
        }

        GameObject controllerObject =
            new GameObject(nameof(CharacterGrowthController));

        _controller =
            controllerObject.AddComponent<CharacterGrowthController>();

        _controller.EnterAsync().Forget();
    }
}