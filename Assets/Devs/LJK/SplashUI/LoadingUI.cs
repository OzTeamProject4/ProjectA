using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoadingUI : BaseUI
{
    [Header("Video")]
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private RawImage _videoImage;

    [Header("Progress")]
    [SerializeField] private LoadingSlider _loadingSlider;
    [SerializeField] private LoadingButton _loadingButton;

    private const int LoadingCompleteDelay = 1000;

    public void Awake()
    {
        UnityUtil.ValidateReference(_videoPlayer, nameof(LoadingUI), nameof(_videoPlayer));
        UnityUtil.ValidateReference(_videoImage, nameof(LoadingUI), nameof(_videoImage));
        UnityUtil.ValidateReference(_loadingSlider, nameof(LoadingUI), nameof(_loadingSlider));
        UnityUtil.ValidateReference(_loadingButton, nameof(LoadingUI), nameof(_loadingButton));
    }

    private void OnEnable()
    {
        StartLoadingAsync().Forget();
    }

    private async UniTask StartLoadingAsync()
    {
        UpdateLoadingState(LoadingState.Loading);

        await PrepareLoadingVideoAsync();
        await RunLoadingAsync();
    }

    private async UniTask RunLoadingAsync()
    {
        IProgress<LoadingProgress> progress = new Progress<LoadingProgress>(OnLoadingProgressChanged);

        await GameManager.Instance.DataManager.LoadRuntimeDataAsync(progress);

        await UniTask.WaitUntil(IsLoadingCompleted);

        await UniTask.Delay(LoadingCompleteDelay);

        UpdateLoadingState(LoadingState.Ready);
    }

    private async UniTask PrepareLoadingVideoAsync()
    {
        AudioSource bgmAudioSource = GameManager.Instance.AudioManager.GetBgmAudioSource();

        if (bgmAudioSource == null)
        {
            Debug.LogError($"[{nameof(LoadingUI)}:{nameof(PrepareLoadingVideoAsync)}] BGM AudioSource를 찾을 수 없습니다.");
            return;
        }

        VideoClip videoClip = await GameManager.Instance.ResourceManager.LoadAssetAsync<VideoClip>(AddressableKey.Asset.LoadingVideoClip);

        if (videoClip == null)
        {
            Debug.LogError($"[{nameof(LoadingUI)}:{nameof(PrepareLoadingVideoAsync)}] VideoClip을 로드하지 못했습니다.");
            return;
        }

        _videoPlayer.clip = videoClip;
        _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        _videoPlayer.SetTargetAudioSource(0, bgmAudioSource);

        _videoPlayer.prepareCompleted -= OnVideoPrepared;
        _videoPlayer.prepareCompleted += OnVideoPrepared;

        _videoPlayer.Prepare();
    }

    private void OnLoadingProgressChanged(LoadingProgress progress)
    {
        _loadingSlider.UpdateProgress(progress);
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        source.prepareCompleted -= OnVideoPrepared;

        source.Play();

        GameManager.Instance.UIManager.CloseOverlayUI();
    }

    private void UpdateLoadingState(LoadingState loadingState)
    {
        switch (loadingState)
        {
            case LoadingState.Loading:
                _loadingSlider.gameObject.SetActive(true);
                _loadingButton.gameObject.SetActive(false);
                break;

            case LoadingState.Ready:
                _loadingSlider.gameObject.SetActive(false);
                _loadingButton.gameObject.SetActive(true);
                break;
        }
    }

    private bool IsLoadingCompleted()
    {
        bool isLoadingCompleted = _loadingSlider.IsCompleted;
        return isLoadingCompleted;
    }
}
