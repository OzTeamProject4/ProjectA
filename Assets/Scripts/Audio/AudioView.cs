using UnityEngine;

public class AudioView : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourceBGM;
    [SerializeField] private AudioSource _audioSourceSFX;

    private AudioViewModel _audioViewModel;

    private void Awake()
    {
        UnityUtil.ValidateReference(_audioSourceBGM, nameof(AudioClip), nameof(_audioSourceBGM));
        UnityUtil.ValidateReference(_audioSourceSFX, nameof(AudioClip), nameof(_audioSourceSFX));

        _audioViewModel = new AudioViewModel();
        _audioViewModel.ModelPropertyChanged += OnModelPropertyChanged;
        _audioViewModel.RequestRefreshProperties();
    }

    private void OnDestroy()
    {
        _audioViewModel.Dispose();
        _audioViewModel.ModelPropertyChanged -= OnModelPropertyChanged;
        _audioViewModel = null;
    }

    public AudioSource GetBgmAudioSource()
    {
        return _audioSourceBGM;
    }

    public void PlayBGM(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            Debug.LogWarning($"[{nameof(AudioView)}:{nameof(PlayBGM)}] AudioClip이 null입니다. BGM을 재생할 수 없습니다.");
            return;
        }

        SetBGMClip(audioClip);
        _audioSourceBGM.Play();
    }

    public void StopBGM()
    {
        _audioSourceBGM.Stop();
    }

    public void PlaySFX(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            Debug.LogWarning($"[{nameof(AudioView)}:{nameof(PlaySFX)}] AudioClip이 null입니다. SFX를 재생할 수 없습니다.");
            return;
        }

        _audioSourceSFX.PlayOneShot(audioClip);
    }

    public void SetBgmVolume(float volume)
    {
        _audioViewModel.RequestSetBgmVolume(volume);
    }

    public void SetSfxVolume(float volume)
    {
        _audioViewModel.RequestSetSfxVolume(volume);
    }

    private void SetBGMClip(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            Debug.LogWarning($"[{nameof(AudioView)}:{nameof(SetBGMClip)}] AudioClip이 null입니다. BGM을 재생할 수 없습니다.");
            return;
        }

        _audioSourceBGM.clip = audioClip;
    }

    private void OnModelPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_audioViewModel.BgmVolume):
                _audioSourceBGM.volume = _audioViewModel.BgmVolume;
                break;
            case nameof(_audioViewModel.SfxVolume):
                _audioSourceSFX.volume = _audioViewModel.SfxVolume;
                break;
        }
    }
}