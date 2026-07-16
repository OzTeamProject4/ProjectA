using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MainDungeonButtonView : MonoBehaviour
{
    private Button _button;

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

        if (GameManager.Instance.UIManager == null)
        {
            Debug.LogError("UIManager is null.");
            return;
        }

        OpenStageSelectScreenAsync(destroyCancellationToken).Forget();
    }

    private async UniTask OpenStageSelectScreenAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await GameManager.Instance.UIManager
                .OpenStageSelectScreenAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Failed to open StageSelectScreen.\n{exception}");
        }
    }
}