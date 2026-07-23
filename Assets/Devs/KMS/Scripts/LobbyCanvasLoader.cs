using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class LobbyCanvasLoader : MonoBehaviour
{
    private const int ManagerInitializationDelayFrames = 2;

    private void Start()
    {
        LoadLobbyCanvasAsync(destroyCancellationToken).Forget();
    }

    private async UniTask LoadLobbyCanvasAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await UniTask.WaitUntil(
                () =>
                    GameManager.Instance != null &&
                    GameManager.Instance.UIManager != null,
                cancellationToken: cancellationToken);

            await UniTask.DelayFrame(
                ManagerInitializationDelayFrames,
                cancellationToken: cancellationToken);

            await GameManager.Instance.UIManager.OpenLobbyAsync(
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"[LobbyCanvasLoader] 로비 UI 로드 실패: {exception}");
        }
    }
}