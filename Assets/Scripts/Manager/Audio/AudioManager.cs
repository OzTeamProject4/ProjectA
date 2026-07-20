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
            await CreateAudioView();
        }
    }

    public AudioSource GetBgmAudioSource()
    {
        return _audioView.GetBgmAudioSource();
    }

    public void PlayBGM(string audioId)
    {
        if (string.IsNullOrWhiteSpace(audioId))
        {
            Debug.LogError($"[AudioManager:PlayBGM] audioId가 null이거나 비어 있습니다.");
            return;
        }

        if (!_audioClipDictionary.TryGetValue(audioId, out AudioClip audioClip))
        {
            Debug.LogError($"[AudioManager:PlayBGM] '{audioId}'에 해당하는 AudioClip을 찾을 수 없습니다.");
        }

        _audioView.PlayBGM(audioClip);
    }

    public void StopBGM()
    {
        _audioView.StopBGM();
    }

    public void PlaySFX(string audioId)
    {
        if (string.IsNullOrWhiteSpace(audioId))
        {
            Debug.LogError($"[{nameof(AudioManager)}:{nameof(PlaySFX)}] audioId가 null이거나 비어 있습니다.");
            return;
        }

        if (!_audioClipDictionary.TryGetValue(audioId, out AudioClip audioClip))
        {
            Debug.LogError($"[{nameof(AudioManager)}:{nameof(PlaySFX)}] '{audioId}'에 해당하는 AudioClip을 찾을 수 없습니다.");
        }

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

    public async UniTask LoadAudioClipsAsync()
    {
        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, AudioData> audioDataTable))
        {
            Debug.LogError($"[{nameof(AudioManager)}:{nameof(LoadAudioClipsAsync)}] 오디오 데이터 테이블을 가져오지 못했습니다.");
            return;
        }

        foreach (AudioData audioData in audioDataTable.Values)
        {
            string audioDataId = audioData.DataId;
            string audioClipKey = audioData.AudioClipKey;

            AudioClip audioClip = await GameManager.Instance.ResourceManager.LoadAssetAsync<AudioClip>(audioClipKey, destroyCancellationToken);

            if (audioClip == null)
            {
                Debug.LogError($"[{nameof(AudioManager)}:{nameof(LoadAudioClipsAsync)}] {audioClipKey} 오디오 에셋을 로드하지 못했습니다.");
                return;
            }

            _audioClipDictionary[audioDataId] = audioClip;
        }
    }

    private async UniTask CreateAudioView()
    {
        GameObject audioControllerPrefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(AddressableKey.Prefab.AudioView, destroyCancellationToken);

        if (audioControllerPrefab == null)
        {
            Debug.LogError($"[{nameof(AudioManager)}:{nameof(CreateAudioView)}] {AddressableKey.Prefab.AudioView} 오디오 뷰를 로드하지 못했습니다.");
            return;
        }

        if (!audioControllerPrefab.TryGetComponent(out AudioView audioView))
        {
            Debug.LogError($"[{nameof(AudioManager)}:{nameof(CreateAudioView)}] AudioView 프리팹에 AudioView 컴포넌트가 없습니다.");
            return;
        }

        _audioView = Instantiate(audioView);
        _audioView.name = audioView.name;
    }
}