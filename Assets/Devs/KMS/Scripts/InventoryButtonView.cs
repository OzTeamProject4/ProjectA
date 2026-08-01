using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InventoryButtonView : MonoBehaviour
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
            Debug.LogError("Inventory Button component was not found.");
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
        OpenInventoryDetailAsync(cancellationToken).Forget();
    }

    private async UniTaskVoid OpenInventoryDetailAsync(
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

            await GameManager.Instance.UIManager.OpenInventoryDetailAsync(
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Failed to open InventoryScreen: {exception.Message}");
        }
    }
}