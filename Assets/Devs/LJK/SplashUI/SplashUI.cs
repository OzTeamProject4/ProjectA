using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject _splashRoot;
    [SerializeField] private Image _logoImage;

    [Header("Logo")]
    [SerializeField] private List<Sprite> _logoSpriteList;

    [Range(1f, 100f)]
    [SerializeField] private float _logoDisplayDuration;

    private void Awake()
    {
        if (_logoImage == null)
        {
            Debug.LogError($"[{nameof(SplashUI)}] Image 컴포넌트가 할당되지 않았습니다.");
        }
    }

    private void Start()
    {
        PlaySplashSequenceAsync().Forget();
    }

    private async UniTask PlaySplashSequenceAsync()
    {
        UniTask initializeManagersTask = GameManager.Instance.InitializeManagersAsync();

        await PlaySplashAsync();
        await initializeManagersTask;

        await GameManager.Instance.UIManager.OpenOverlayUIAsync();
        await GameManager.Instance.UIManager.OpenLoadingUIAsync();

        Destroy(_splashRoot);
    }

    private async UniTask PlaySplashAsync()
    {
        if (_logoSpriteList == null || _logoSpriteList.Count == 0)
        {
            Debug.LogWarning($"[{nameof(SplashUI)}:{nameof(PlaySplashAsync)}] Logo Sprite가 없습니다.");
            return;
        }

        foreach (Sprite logoSprite in _logoSpriteList)
        {
            _logoImage.sprite = logoSprite;
            await FadeAsync();
        }
    }

    private async UniTask FadeAsync()
    {
        float halfFadeDuration = _logoDisplayDuration * 0.5f;

        await FadeAlphaAsync(0f, 1f, halfFadeDuration);
        await FadeAlphaAsync(1f, 0f, halfFadeDuration);
    }

    private async UniTask FadeAlphaAsync(float fromAlpha, float toAlpha, float duration)
    {
        float fadeTime = 0f;
        Color color = _logoImage.color;

        while (fadeTime < duration)
        {
            fadeTime += Time.deltaTime;

            color.a = Mathf.Lerp(fromAlpha, toAlpha, (fadeTime / duration));
            _logoImage.color = color;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        color.a = toAlpha;
        _logoImage.color = color;
    }
}