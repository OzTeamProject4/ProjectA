using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : BaseManager<DialogueManager>
{
    private DialogueModel _dialogueModel;

    private string _currentStoryKey;
    private string _currentDialogueId;

    private readonly Dictionary<string, DialogueData> _dialogueLines = new Dictionary<string, DialogueData>();
    private readonly Dictionary<string, List<ChoiceData>> _choiceData = new Dictionary<string, List<ChoiceData>>();

    public override UniTask InitializeAsync()
    {
        _dialogueModel = NetworkManager.Instance.DialogueModel;
        return UniTask.CompletedTask;
    }

    public async UniTask StartDialogue(string storyId)
    {
        await LoadDialogueAsync(storyId);

        if (!_dialogueLines.TryGetValue(_currentDialogueId, out DialogueData dialogueData))
        {
            Debug.LogError($"[{nameof(DialogueManager)}:{nameof(StartDialogue)}] '{_currentDialogueId}' 대화 데이터를 찾을 수 없습니다.");
            return;
        }

        _dialogueModel.UpdateDialogue(dialogueData);

        GameManager.Instance.UIManager.OpenDialogueAsync().Forget();
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

        _currentDialogueId = dialogueData.NextDialogueId;

        if (!_dialogueLines.TryGetValue(_currentDialogueId, out DialogueData nextDialogueData))
        {
            Debug.LogError($"[{nameof(DialogueManager)}:{nameof(AdvanceDialogue)}] '{_currentDialogueId}' 대화 데이터를 찾을 수 없습니다.");
            return;
        }

        _dialogueModel.UpdateDialogue(nextDialogueData);
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
        _dialogueModel.SetChoiceOpen(true);
    }

    private void EndDialogue()
    {
        throw new NotImplementedException();
    }

    private async UniTask LoadDialogueAsync(string key)
    {
        if (_currentStoryKey == key)
        {
            return;
        }

        if (!GameManager.Instance.DataManager.TryGetData(key, out StoryInfoData storyInfoData))
        {
            Debug.LogError($"[{nameof(DialogueManager)}:{nameof(LoadDialogueAsync)}] '{key}' 스토리 정보 데이터를 찾을 수 없습니다.");
            return;
        }

        List<DialogueData> dialogueDatas = await LoadDataTableAsync<DialogueData>(storyInfoData.DialogueKey);
        List<ChoiceData> choiceDatas = await LoadDataTableAsync<ChoiceData>(storyInfoData.ChoiceKey);

        CacheDialogueData(dialogueDatas);
        CacheChoiceData(choiceDatas);

        _currentStoryKey = key;
        _currentDialogueId = storyInfoData.StartDialogueId;
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
        _choiceData.Clear();

        if (choiceDatas == null || choiceDatas.Count == 0)
        {
            return;
        }

        foreach (ChoiceData choiceData in choiceDatas)
        {
            if (!_choiceData.TryGetValue(choiceData.DataId, out List<ChoiceData> choices))
            {
                choices = new List<ChoiceData>();
                _choiceData.Add(choiceData.DataId, choices);
            }

            choices.Add(choiceData);
        }
    }
}