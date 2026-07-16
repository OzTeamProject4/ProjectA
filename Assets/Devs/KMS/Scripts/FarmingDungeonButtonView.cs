using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FarmingDungeonButtonView : MonoBehaviour
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

        OpenFarmingDungeonScreenAsync(
            destroyCancellationToken).Forget();
    }

    private async UniTask OpenFarmingDungeonScreenAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await GameManager.Instance.UIManager
                .OpenFarmingDungeonScreenAsync(
                    cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Failed to open FarmingDungeonScreen.\n{exception}");
        }
    }
}