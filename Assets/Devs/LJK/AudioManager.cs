using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : BaseManager<AudioManager>
{
    private AudioView _audioView;

    private readonly Dictionary<string, AudioClip> _audioClipDictionary = new Dictionary<string, AudioClip>();

    public override async UniTask InitializeAsync()
    {
        if (_audioView == null)
        {
            await CreateAudioController();
        }

        await LoadAudioClipsAsync();
    }

    public void PlayBGM(string audioId)
    {
        AudioClip audioClip = _audioClipDictionary[audioId];
        _audioView.PlayBGM(audioClip);
    }

    public void StopBGM()
    {
        _audioView.StopBGM();
    }

    public void PlaySFX(string audioId)
    {
        AudioClip audioClip = _audioClipDictionary[audioId];
        _audioView.PlaySFX(audioClip);
    }

    public void SetBgmVolume(float volume)
    {
        _audioView.SetBgmVolume(volume);
    }

    public void SetSfxVolume(float volume)
    {
        _audioView.SetSfxVolume(volume);
    }


    private async UniTask CreateAudioController()
    {
        GameObject audioControllerPrefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(AddressableKey.Prefab.AudioController);

        if (!audioControllerPrefab.TryGetComponent(out AudioView audioController))
        {
            Debug.LogError("[AudioManager:CreateAudioController] AudioController 프리팹에 AudioController 컴포넌트가 없습니다.");
            return;
        }

        _audioView = Instantiate(audioController);
    }

    private async UniTask LoadAudioClipsAsync()
    {
        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, AudioData> audioDataTable))
        {
            Debug.LogError("[AudioManager:LoadAudioClipsAsync] 오디오 데이터 테이블을 가져오지 못했습니다.");
            return;
        }

        foreach (AudioData audioData in audioDataTable.Values)
        {
            string audioDataId = audioData.DataId;
            string audioClipKey = audioData.AudioClipKey;

            AudioClip audioClip = await GameManager.Instance.ResourceManager.LoadAssetAsync<AudioClip>(audioClipKey);

            if (audioClip == null)
            {
                Debug.LogError($"[AudioManager:LoadAudioClipsAsync] {audioClipKey} 오디오 에셋을 로드하지 못했습니다.");
                return;
            }

            _audioClipDictionary[audioDataId] = audioClip;
        }
    }
}