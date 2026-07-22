using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LobbyCanvasLoader : MonoBehaviour
{
    private const string LobbyCanvasAddress = "UI/KMS_LobbyCanvas";
    private const string UILayerObjectName = "UILayer(Clone)";
    private const string LobbyParentName = "A";

    private GameObject _lobbyCanvasInstance;

    private void Start()
    {
        CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();
        LoadLobbyCanvasAsync(cancellationToken).Forget();
    }

    private void OnDestroy()
    {
        if (_lobbyCanvasInstance == null)
        {
            return;
        }

        Addressables.ReleaseInstance(_lobbyCanvasInstance);
        _lobbyCanvasInstance = null;
    }

    private async UniTaskVoid LoadLobbyCanvasAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            if (_lobbyCanvasInstance != null)
            {
                return;
            }

            Transform parentTransform =
                await FindLobbyParentAsync(cancellationToken);

            if (parentTransform == null)
            {
                Debug.LogError("UILayer A parent was not found.");
                return;
            }

            _lobbyCanvasInstance = await Addressables
                .InstantiateAsync(
                    LobbyCanvasAddress,
                    parentTransform,
                    false)
                .ToUniTask(cancellationToken: cancellationToken);

            ResetRectTransform();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Failed to load KMS_LobbyCanvas: {exception.Message}");
        }
    }

    private async UniTask<Transform> FindLobbyParentAsync(
        CancellationToken cancellationToken)
    {
        GameObject uiLayerObject = null;

        await UniTask.WaitUntil(
            () =>
            {
                uiLayerObject = GameObject.Find(UILayerObjectName);
                return uiLayerObject != null;
            },
            cancellationToken: cancellationToken);

        Transform parentTransform =
            uiLayerObject.transform.Find(LobbyParentName);

        return parentTransform;
    }

    private void ResetRectTransform()
    {
        if (_lobbyCanvasInstance.transform is not RectTransform rectTransform)
        {
            Debug.LogError("KMS_LobbyCanvas RectTransform was not found.");
            return;
        }

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }
}