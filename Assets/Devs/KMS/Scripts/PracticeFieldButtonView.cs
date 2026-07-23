using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class PracticeFieldButtonView : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnPracticeFieldButtonClicked);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnPracticeFieldButtonClicked);
    }

    private void OnPracticeFieldButtonClicked()
    {
        OpenPracticeFieldScreenAsync().Forget();
    }

    private async UniTaskVoid OpenPracticeFieldScreenAsync()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[PracticeFieldButtonView] GameManager가 존재하지 않습니다.");
            return;
        }

        if (GameManager.Instance.UIManager == null)
        {
            Debug.LogError("[PracticeFieldButtonView] UIManager가 존재하지 않습니다.");
            return;
        }

        await GameManager.Instance.UIManager.OpenPracticeFieldScreenAsync(
            destroyCancellationToken);
    }
}