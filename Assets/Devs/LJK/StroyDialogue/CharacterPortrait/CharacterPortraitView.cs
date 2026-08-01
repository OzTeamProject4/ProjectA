using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraitView : MonoBehaviour
{
    private static readonly Color ActiveColor = Color.white;
    private static readonly Color InactiveColor = Color.gray;

    [SerializeField] private Image _portraitImage;

    private string _currentPortraitId;

    private CancellationTokenSource _disableCts;

    private void OnDisable()
    {
        if (_disableCts == null)
        {
            return;
        }

        _disableCts.Cancel();
        _disableCts.Dispose();
        _disableCts = null;
    }

    public void SetPortrait(string dataId)
    {
        if (string.IsNullOrWhiteSpace(dataId))
        {
            gameObject.SetActive(false);
            return;
        }

        if (_currentPortraitId == dataId)
        {
            return;
        }

        _currentPortraitId = dataId;
        LoadPortraitAsync(dataId).Forget();
    }

    public void UpdateState(string activeCharacterId)
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        _portraitImage.color = (_currentPortraitId == activeCharacterId) ? ActiveColor : InactiveColor;
    }

    private async UniTask LoadPortraitAsync(string dataId)
    {
        if (string.IsNullOrWhiteSpace(dataId))
        {
            Debug.LogError($"[{nameof(CharacterPortraitView)}:{nameof(LoadPortraitAsync)}] 전달된 portrait key가 null이거나 빈 문자열입니다.");
            return;
        }

        if(!GameManager.Instance.DataManager.TryGetData(dataId, out DialoguePortraitData dialoguePortraitData))
        {
            Debug.LogError($"[{nameof(CharacterPortraitView)}:{nameof(LoadPortraitAsync)}] '{dataId}' DialoguePortraitData를 찾을 수 없습니다.");
            return;
        }

        if (_disableCts == null)
        {
            _disableCts = new CancellationTokenSource();
        }
 
        Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(dialoguePortraitData.PortraitKey, _disableCts.Token);

        if (sprite == null)
        {
            Debug.LogError($"[{nameof(CharacterPortraitView)}:{nameof(LoadPortraitAsync)}] '{dialoguePortraitData.PortraitKey}' 경로의 Sprite를 불러오지 못했습니다.");
            return;
        }

        _portraitImage.sprite = sprite;
        gameObject.SetActive(true);
    }
}