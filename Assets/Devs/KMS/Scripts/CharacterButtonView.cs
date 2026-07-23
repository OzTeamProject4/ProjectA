using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CharacterButtonView : MonoBehaviour
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
        CancellationToken cancellationToken =
            this.GetCancellationTokenOnDestroy();

        OpenCharacterListAsync(cancellationToken).Forget();
    }

    private async UniTaskVoid OpenCharacterListAsync(
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

            await GameManager.Instance.UIManager.OpenCharacterListAsync(
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Failed to open CharacterList.\n{exception}");
        }
    }
}
