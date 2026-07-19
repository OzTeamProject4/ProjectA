using System.ComponentModel;

public class AudioModel : DataModel, INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs BgmVolumeChanged = new PropertyChangedEventArgs(nameof(BgmVolume));
    private static readonly PropertyChangedEventArgs SfxVolumeChanged = new PropertyChangedEventArgs(nameof(SfxVolume));

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
                OnPropertyChanged(BgmVolumeChanged);
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
                OnPropertyChanged(SfxVolumeChanged);
            }
        }
    }

    public void RefreshProperties()
    {
        OnPropertyChanged(BgmVolumeChanged);
        OnPropertyChanged(SfxVolumeChanged);
    }

    public void SetBgmVolume(float volume)
    {
        BgmVolume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        SfxVolume = volume;
    }

    private void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(this, propertyChangedEventArgs);
    }
}