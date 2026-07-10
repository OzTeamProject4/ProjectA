using System;
using System.ComponentModel;

public class AudioViewModel
{
    private readonly AudioModel _audioModel;

    public AudioViewModel(AudioSettingData audioSettingData)
    {
        _audioModel = new AudioModel(audioSettingData);
        _audioModel.PropertyChanged += OnModelPropertyChanged;
    }

    public event Action<string> ModelPropertyChanged;

    public float BgmVolume
    {
        get
        {
            return _audioModel.BgmVolume;
        }
    }

    public float SfxVolume
    {
        get
        {
            return _audioModel.SfxVolume;
        }
    }

    public void RequestSetBgmVolume(float volume)
    {
        _audioModel.SetBgmVolume(volume);
    }

    public void RequestSetSfxVolume(float volume)
    {
        _audioModel.SetSfxVolume(volume);
    }

    private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        ModelPropertyChanged?.Invoke(e.PropertyName);
    }
}