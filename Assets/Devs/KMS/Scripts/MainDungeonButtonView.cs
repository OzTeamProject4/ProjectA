using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class MainDungeonButtonView : MonoBehaviour
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
                "[MainDungeonButtonView] GameManager.Instance가 존재하지 않습니다.");
            return;
        }

        UIManager uiManager = GameManager.Instance.UIManager;

        if (uiManager == null)
        {
            Debug.LogError(
                "[MainDungeonButtonView] UIManager가 존재하지 않습니다.");
            return;
        }

        OpenStageSelectScreenAsync(
            uiManager,
            destroyCancellationToken).Forget();
    }

    private async UniTask OpenStageSelectScreenAsync(
        UIManager uiManager,
        CancellationToken cancellationToken)
    {
        try
        {
            await uiManager.OpenStageSelectScreenAsync(
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
                $"[MainDungeonButtonView] StageSelectScreen을 열지 못했습니다.\n{exception}");
        }
    }
}