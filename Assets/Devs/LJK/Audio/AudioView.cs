using UnityEngine;

public class AudioView : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourceBGM;
    [SerializeField] private AudioSource _audioSourceSFX;

    private AudioViewModel _audioViewModel;

    private void Awake()
    {
        AudioSettingData audioSettingData = CreateAudioData();
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
            Debug.LogWarning($"[AudioView:PlayBGM] AudioClip이 null입니다. BGM을 재생할 수 없습니다.");
            return;
        }

        ChangeBGMClip(audioClip);
        PlayBGM();
    }
    public void StopBGM()
    {
        _audioSourceBGM.Stop();
    }

    public void PlaySFX(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            Debug.LogWarning($"[AudioView:PlaySFX] AudioClip이 null입니다. SFX를 재생할 수 없습니다.");
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

    private void PlayBGM()
    {
        _audioSourceBGM.Play();
    }

    private void ChangeBGMClip(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            Debug.LogWarning($"[AudioView:ChangeBGMClip] AudioClip이 null입니다. BGM을 재생할 수 없습니다.");
            return;
        }

        _audioSourceBGM.clip = audioClip;
    }

    private AudioSettingData CreateAudioData()
    {
        if (_audioSourceBGM == null)
        {
            Debug.LogError("[AudioView:CreateAudioData] BGM AudioSource가 없습니다.");
            return null;
        }

        if (_audioSourceSFX == null)
        {
            Debug.LogError("[AudioView:CreateAudioData] SFX AudioSource가 없습니다.");
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
                Debug.Log($"{_audioSourceBGM.volume}변경");
                break;
            case nameof(_audioViewModel.SfxVolume):
                _audioSourceSFX.volume = _audioViewModel.SfxVolume;
                break;
        }
    }
}