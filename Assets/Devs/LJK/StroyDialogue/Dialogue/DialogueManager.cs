using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DialogueManager : BaseManager<DialogueManager>
{
    private DialogueModel _dialogueModel;

    private string _currentStoryKey;
    private string _currentDialogueId;

    private readonly Dictionary<string, DialogueData> _dialogueLines = new Dictionary<string, DialogueData>();
    private readonly Dictionary<string, List<ChoiceData>> _choiceDataDictionary = new Dictionary<string, List<ChoiceData>>();

    private CancellationTokenSource _autoModeCts;

    private UniTaskCompletionSource _dialogueCompletionSource;

    public override UniTask InitializeAsync()
    {
        _dialogueModel = NetworkManager.Instance.DialogueModel;
        return UniTask.CompletedTask;
    }

    public async UniTask StartDialogue(string storyId)
    {
        await GameManager.Instance.UIManager.OpenOverlayAsync();

        try
        {
            string startDialogueId = await LoadDialogueAsync(storyId);

            if (string.IsNullOrWhiteSpace(startDialogueId))
            {
                Debug.LogError($"[{nameof(DialogueManager)}:{nameof(StartDialogue)}] '{storyId}' 스토리의 시작 대화 ID를 가져오지 못했습니다.");
                return;
            }

            _dialogueModel.Initialize();
            ChangeDialogue(startDialogueId);

            _dialogueCompletionSource = new UniTaskCompletionSource();

            await GameManager.Instance.UIManager.OpenDialogueAsync();
        }
        finally
        {
            GameManager.Instance.UIManager.CloseOverlay();
        }

        await _dialogueCompletionSource.Task;
    }

    public void AdvanceDialogue()
    {
        if (!_dialogueLines.TryGetValue(_currentDialogueId, out DialogueData dialogueData))
        {
            Debug.LogError($"[{nameof(DialogueManager)}:{nameof(AdvanceDialogue)}] '{_currentDialogueId}' 대화 데이터를 찾을 수 없습니다.");
            return;
        }

        if (HasChoiceGroup(dialogueData))
        {
            EnterChoicePhase(dialogueData.ChoiceGroupId);
            return;
        }

        if (!HasNextDialogue(dialogueData))
        {
            EndDialogue();
            return;
        }

        ChangeDialogue(dialogueData.NextDialogueId);
    }

    public void SelectChoice(string nextDialogueId)
    {
        _dialogueModel.SetChoiceOpen(false);

        if (string.IsNullOrEmpty(nextDialogueId))
        {
            EndDialogue();
            return;
        }

        ChangeDialogue(nextDialogueId);
    }

    public void RequestToggleAutoMode()
    {
        _dialogueModel.SetAutoMode(!_dialogueModel.IsAutoMode);

        if (_dialogueModel.IsAutoMode)
        {
            StartAutoMode();
            return;
        }

        StopAutoMode();
    }

    public void SkipDialogue()
    {
        EndDialogue();
    }

    public void OpenHistoryDialogue()
    {
        StopAutoMode();

        GameManager.Instance.UIManager.OpenDialogueHistoryAsync().Forget();
    }

    public void HideDialogue()
    {
        StopAutoMode();
    }

    private void StartAutoMode()
    {
        if (_autoModeCts != null)
        {
            _autoModeCts.Cancel();
            _autoModeCts.Dispose();
        }

        _autoModeCts = new CancellationTokenSource();

        _dialogueModel.SetAutoMode(true);

        RunAutoModeAsync(_autoModeCts.Token).Forget();
    }

    private void StopAutoMode()
    {
        _dialogueModel.SetAutoMode(false);

        if (_autoModeCts != null)
        {
            _autoModeCts.Cancel();
            _autoModeCts.Dispose();
            _autoModeCts = null;
        }
    }

    private async UniTask RunAutoModeAsync(CancellationToken cancellationToken)
    {
        try
        {
            AdvanceDialogue();

            while (_dialogueModel.IsAutoMode)
            {
                if (!_dialogueLines.TryGetValue(_currentDialogueId, out DialogueData dialogueData))
                {
                    Debug.LogError($"[{nameof(DialogueManager)}:{nameof(RunAutoModeAsync)}] '{_currentDialogueId}' 대화 데이터를 찾을 수 없습니다.");
                    return;
                }

                if (dialogueData.AutoNextDelay <= 0)
                {
                    StopAutoMode();
                    return;
                }

                await UniTask.Delay(TimeSpan.FromSeconds(dialogueData.AutoNextDelay), cancellationToken: cancellationToken);

                if (!_dialogueModel.IsAutoMode)
                {
                    return;
                }

                AdvanceDialogue();
            }
        }
        catch (OperationCanceledException) { }
    }

    private void ChangeDialogue(string dialogueId)
    {
        if (string.IsNullOrWhiteSpace(dialogueId))
        {
            Debug.LogError($"[{nameof(DialogueManager)}:{nameof(ChangeDialogue)}] 전달된 dialogueId가 null이거나 빈 문자열 또는 공백 문자열입니다.");
            return;
        }

        _currentDialogueId = dialogueId;

        if (!_dialogueLines.TryGetValue(_currentDialogueId, out DialogueData nextDialogueData))
        {
            Debug.LogError($"[{nameof(DialogueManager)}:{nameof(AdvanceDialogue)}] '{_currentDialogueId}' 대화 데이터를 찾을 수 없습니다.");
            return;
        }

        _dialogueModel.UpdateDialogue(nextDialogueData);

        if (!string.IsNullOrWhiteSpace(nextDialogueData.Bgm))
        {
            GameManager.Instance.AudioManager.PlayBGM(nextDialogueData.Bgm);
        }
    }

    private bool HasChoiceGroup(DialogueData dialogueData)
    {
        return !string.IsNullOrWhiteSpace(dialogueData.ChoiceGroupId);
    }

    private bool HasNextDialogue(DialogueData dialogueData)
    {
        return !string.IsNullOrWhiteSpace(dialogueData.NextDialogueId);
    }

    private void EnterChoicePhase(string choiceGroupId)
    {
        if (!_choiceDataDictionary.TryGetValue(choiceGroupId, out List<ChoiceData> choiceDatas))
        {
            Debug.LogError($"[{nameof(DialogueManager)}:{nameof(EnterChoicePhase)}] '{choiceGroupId}' 선택지 데이터를 찾을 수 없습니다.");
            return;
        }

        _dialogueModel.SetChoices(choiceDatas);
        _dialogueModel.SetChoiceOpen(true);
    }

    private void EndDialogue()
    {
        StopAutoMode();

        GameManager.Instance.UIManager.CloseDialogue();

        _dialogueCompletionSource?.TrySetResult();
        _dialogueCompletionSource = null;
    }

    private async UniTask<string> LoadDialogueAsync(string key)
    {
        if (!GameManager.Instance.DataManager.TryGetData(key, out StoryInfoData storyInfoData))
        {
            Debug.LogError($"[{nameof(DialogueManager)}:{nameof(LoadDialogueAsync)}] '{key}' 스토리 정보 데이터를 찾을 수 없습니다.");
            return null;
        }

        if (_currentStoryKey != key)
        {
            List<DialogueData> dialogueDatas = await LoadDataTableAsync<DialogueData>(storyInfoData.DialogueKey);
            List<ChoiceData> choiceDatas = await LoadDataTableAsync<ChoiceData>(storyInfoData.ChoiceKey);

            CacheDialogueData(dialogueDatas);
            CacheChoiceData(choiceDatas);

            _currentStoryKey = key;
        }

        return storyInfoData.StartDialogueId;
    }

    private async UniTask<List<T>> LoadDataTableAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError($"[{nameof(DialogueManager)}:{nameof(LoadDataTableAsync)}] 전달된 key가 null이거나 빈 문자열 또는 공백 문자열입니다.");
            return null;
        }

        try
        {
            TextAsset jsonTextAsset = await GameManager.Instance.ResourceManager.LoadAssetAsync<TextAsset>(key, destroyCancellationToken);

            if (jsonTextAsset == null)
            {
                throw new InvalidOperationException($"'{key}' 데이터 TextAsset 로드에 실패했습니다.");
            }

            List<T> datas = JsonConvert.DeserializeObject<List<T>>(jsonTextAsset.text);

            if (datas == null)
            {
                throw new InvalidOperationException($"'{key}' JSON 역직렬화 결과가 null입니다.");
            }

            return datas;
        }
        catch (JsonException exception)
        {
            Debug.LogError($"[{nameof(DialogueManager)}:{nameof(LoadDataTableAsync)}] JSON 파싱에 실패했습니다.\n{exception}");
            return null;
        }
        catch (Exception exception)
        {
            Debug.LogError($"[{nameof(DialogueManager)}:{nameof(LoadDataTableAsync)}] 데이터 테이블 로드 중 오류가 발생했습니다.\n{exception}");
            return null;
        }
    }

    private void CacheDialogueData(IReadOnlyList<DialogueData> dialogueDatas)
    {
        _dialogueLines.Clear();

        if (dialogueDatas == null || dialogueDatas.Count == 0)
        {
            return;
        }

        foreach (DialogueData dialogueData in dialogueDatas)
        {
            if (!_dialogueLines.TryAdd(dialogueData.DataId, dialogueData))
            {
                Debug.LogError($"[{nameof(DialogueManager)}:{nameof(CacheDialogueData)}] '{dialogueData.DataId}' 중복된 데이터 ID가 존재합니다.");
            }
        }
    }

    private void CacheChoiceData(IReadOnlyList<ChoiceData> choiceDatas)
    {
        _choiceDataDictionary.Clear();

        if (choiceDatas == null || choiceDatas.Count == 0)
        {
            return;
        }

        foreach (ChoiceData choiceData in choiceDatas)
        {
            if (!_choiceDataDictionary.TryGetValue(choiceData.DataId, out List<ChoiceData> choices))
            {
                choices = new List<ChoiceData>();
                _choiceDataDictionary.Add(choiceData.DataId, choices);
            }

            choices.Add(choiceData);
        }
    }
}