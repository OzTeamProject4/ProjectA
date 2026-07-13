using UnityEngine;

public class AudioView : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourceBGM;
    [SerializeField] private AudioSource _audioSourceSFX;

    private AudioViewModel _audioViewModel;

    private void Awake()
    {
        AudioSettingData audioSettingData = CreateAudioSettingData();
        _audioViewModel = new AudioViewModel(audioSettingData);
    }

    private void OnEnable()
    {
        _audioViewModel.ModelPropertyChanged += OnModelPropertyChanged;
    }

    private void OnDisable()
    {
        _audioViewModel.ModelPropertyChanged -= OnModelPropertyChanged;
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

    private AudioSettingData CreateAudioSettingData()
    {
        if (_audioSourceBGM == null)
        {
            Debug.LogError($"[{nameof(AudioView)}:{nameof(CreateAudioSettingData)}] BGM AudioSource가 없습니다.");
            return null;
        }

        if (_audioSourceSFX == null)
        {
            Debug.LogError($"[{nameof(AudioView)}:{nameof(CreateAudioSettingData)}] SFX AudioSource가 없습니다.");
            return null;
        }

        AudioSettingData audioSettingData = new AudioSettingData(_audioSourceBGM.volume, _audioSourceSFX.volume);
        return audioSettingData;
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