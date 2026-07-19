using System;
using System.ComponentModel;
using UnityEngine;

public class AudioViewModel
{
    private AudioModel _audioModel;

    public AudioViewModel()
    {
        _audioModel = GameManager.Instance.NetworkManager.ModelContainer.GetModel<AudioModel>();
        _audioModel.PropertyChanged += OnModelPropertyChanged;
    }

    public void Dispose()
    {
        _audioModel.PropertyChanged -= OnModelPropertyChanged;
        _audioModel = null;
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

    public void RequestRefreshProperties()
    {
        _audioModel.RefreshProperties();
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