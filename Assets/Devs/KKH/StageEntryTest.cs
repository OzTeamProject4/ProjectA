using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StageEntryTest : MonoBehaviour
{
    [SerializeField] private StageSceneLoader _loaderPrefab;
    [SerializeField] private Button _enterButton;

    private StageSceneLoader _loader;

    private void OnEnable()
    {
        if (null != _enterButton)
        {
            _enterButton.onClick.AddListener(HandleEnterButtonClicked);
        }
    }

    private void OnDisable()
    {
        if (null != _enterButton)
        {
            _enterButton.onClick.RemoveListener(HandleEnterButtonClicked);
        }
    }

    private void HandleEnterButtonClicked()
    {
        EnterAsync().Forget();
    }

    private async UniTaskVoid EnterAsync()
    {
        if (null != _loader)
        {
            Debug.LogWarning("[StageEntryTest] 이미 진입했습니다.");
            return;
        }

        if (null == _loaderPrefab)
        {
            Debug.LogError("[StageEntryTest] _loaderPrefab 이 연결되지 않았습니다.");
            return;
        }

        _loader = Instantiate(_loaderPrefab);

        if (null != _enterButton)
        {
            _enterButton.transform.parent.gameObject.SetActive(false);
        }

        await _loader.EnterAsync();
    }
}