using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MissionButtonView : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (_button == null)
        {
            Debug.LogError("Mission button component was not found.");
            return;
        }

        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable()
    {
        if (_button == null)
        {
            return;
        }

        _button.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();
        OpenMissionScreenAsync(cancellationToken).Forget();
    }

    private async UniTaskVoid OpenMissionScreenAsync(
        CancellationToken cancellationToken)
    {
        try
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

            await GameManager.Instance.UIManager.OpenMissionScreenAsync(
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Failed to open MissionScreen: {exception.Message}");
        }
    }
}