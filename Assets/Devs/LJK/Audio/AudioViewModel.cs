using System;
using System.ComponentModel;
using UnityEngine;

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
        float clampedVolume = Mathf.Clamp01(volume);
        _audioModel.SetBgmVolume(clampedVolume);
    }

    public void RequestSetSfxVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        _audioModel.SetSfxVolume(clampedVolume);
    }

    private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (ModelPropertyChanged == null)
        {
            return;
        }

        ModelPropertyChanged.Invoke(e.PropertyName);
    }
}