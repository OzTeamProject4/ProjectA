using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class StudentGachaListSlotView : BaseButton
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _portraitImage;

    private string _slotGachaId;

    private CancellationTokenSource _loadCts;

    public event Action<string> ButtonClicked;

    protected override void OnDisable()
    {
        base.OnDisable();

        if (_loadCts != null)
        {
            _loadCts.Cancel();
            _loadCts.Dispose();
            _loadCts = null;
        }
    }

    public async UniTask UpdateGachaBannerSlotAsync(string gachaId, string backgroundKey, string portraitKey)
    {
        _slotGachaId = gachaId;

        if (_loadCts != null)
        {
            _loadCts.Cancel();
            _loadCts.Dispose();
        }

        _loadCts = new CancellationTokenSource();

        (Sprite backgroundSprite, Sprite portraitSprite) =  await UniTask.WhenAll(LoadSpriteAsync(backgroundKey, _loadCts.Token), LoadSpriteAsync(portraitKey, _loadCts.Token));

        _backgroundImage.sprite = backgroundSprite;
        _portraitImage.sprite = portraitSprite;
    }

    public void SetActive(bool active)
    {
        if (gameObject.activeSelf == active)
        {
            return;
        }

        gameObject.SetActive(active);
    }

    protected override void OnButtonClick()
    {
        if (ButtonClicked == null)
        {
            return;
        }

        ButtonClicked.Invoke(_slotGachaId);
    }

    private async UniTask<Sprite> LoadSpriteAsync(string key, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError($"[{nameof(StudentGachaListSlotView)}:{nameof(LoadSpriteAsync)}] 전달된 Sprite key가 null이거나 빈 문자열입니다.");
            return null;
        }

        Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(key, cancellationToken);

        if (sprite == null)
        {
            Debug.LogError($"[{nameof(StudentGachaListSlotView)}:{nameof(LoadSpriteAsync)}] '{key}' Sprite를 찾을 수 없습니다.");
            return null;
        }

        return sprite;
    }
}
