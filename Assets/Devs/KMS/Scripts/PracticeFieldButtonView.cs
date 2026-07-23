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
        EnterStageAsync().Forget();
    }

    private async UniTaskVoid EnterStageAsync()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[PracticeFieldButtonView] GameManager�� �������� �ʽ��ϴ�.");
            return;
        }

        if (GameManager.Instance.UIManager == null)
        {
            Debug.LogError("[PracticeFieldButtonView] UIManager�� �������� �ʽ��ϴ�.");
            return;
        }

        GameManager.Instance.UIManager.CloseLobby();

        await GameManager.Instance.StageManager.EnterAsync();
    }
}