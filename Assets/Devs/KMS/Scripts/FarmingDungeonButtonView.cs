using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class FarmingDungeonButtonView : MonoBehaviour
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
            Debug.LogError(
                "[FarmingDungeonButtonView] GameManager.Instance가 존재하지 않습니다.");
            return;
        }

        UIManager uiManager = GameManager.Instance.UIManager;

        if (uiManager == null)
        {
            Debug.LogError(
                "[FarmingDungeonButtonView] UIManager가 존재하지 않습니다.");
            return;
        }

        OpenFarmingDungeonScreenAsync(
            uiManager,
            destroyCancellationToken).Forget();
    }

    private async UniTask OpenFarmingDungeonScreenAsync(
        UIManager uiManager,
        CancellationToken cancellationToken)
    {
        try
        {
            await uiManager.OpenFarmingDungeonScreenAsync(
                cancellationToken);

            uiManager.ClosePracticeFieldScreen();
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"[FarmingDungeonButtonView] FarmingDungeonScreen을 열지 못했습니다.\n{exception}");
        }
    }
}