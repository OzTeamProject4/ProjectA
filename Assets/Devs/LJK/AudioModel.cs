using System.ComponentModel;

public class AudioModel : INotifyPropertyChanged
{
    private float _bgmVolume;
    private float _sfxVolume;

    public AudioModel(AudioSettingData audioSettingData)
    {
        _bgmVolume = audioSettingData.BgmVolume;
        _sfxVolume = audioSettingData.SfxVolume;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public float BgmVolume
    {
        get { return _bgmVolume; }
        private set
        {
            if (_bgmVolume != value)
            {
                _bgmVolume = value;
                OnPropertyChanged(nameof(BgmVolume));
            }
        }
    }

    public float SfxVolume
    {
        get { return _sfxVolume; }
        private set
        {
            if (_sfxVolume != value)
            {
                _sfxVolume = value;
                OnPropertyChanged(nameof(SfxVolume));
            }
        }
    }

    public void SetBgmVolume(float volume)
    {
        BgmVolume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        SfxVolume = volume;
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}