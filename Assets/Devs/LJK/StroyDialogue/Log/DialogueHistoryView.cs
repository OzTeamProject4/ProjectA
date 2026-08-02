using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueHistoryView : BaseUI
{
    private const int InitialSlotCount = 30;
    
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private DialogueHistorySlotView _slotPrefab;
    [SerializeField] private Transform _content;

    private DialogueHistoryViewModel _dialogueHistoryViewModel;

    private readonly List<DialogueHistorySlotView> _spawnedSlotList = new List<DialogueHistorySlotView>();

    private void Awake()
    {
        UnityUtil.ValidateReference(_slotPrefab, nameof(DialogueHistoryView), nameof(_slotPrefab));
        UnityUtil.ValidateReference(_content, nameof(DialogueHistoryView), nameof(_content));

        PrewarmSlots();

        _dialogueHistoryViewModel = new DialogueHistoryViewModel();
    }

    private void OnEnable()
    {
        RefreshSlots();
        RefreshScrollPosition().Forget();
    }

    private void OnDestroy()
    {
        _dialogueHistoryViewModel.Dispose();
        _dialogueHistoryViewModel = null;
    }

    private void PrewarmSlots()
    {
        for (int i = 0; i < InitialSlotCount; i++)
        {
            DialogueHistorySlotView dialogueHistorySlotView = Instantiate(_slotPrefab, _content);

            dialogueHistorySlotView.gameObject.SetActive(false);

            _spawnedSlotList.Add(dialogueHistorySlotView);
        }
    }

    private void RefreshSlots()
    {
        IReadOnlyList<DialogueData> dialogueHistory = _dialogueHistoryViewModel.DialogueHistory;
        
        if (dialogueHistory == null)
        {
            Debug.LogError($"[{nameof(DialogueHistoryView)}:{nameof(RefreshSlots)}] DialogueHistory가 null입니다.");
            return;
        }

        for (int index = 0; index < dialogueHistory.Count; index++)
        {
            if (index >= _spawnedSlotList.Count)
            {
                DialogueHistorySlotView newDialogueHistorySlotView = Instantiate(_slotPrefab, _content);
                newDialogueHistorySlotView.gameObject.SetActive(false);

                _spawnedSlotList.Add(newDialogueHistorySlotView);
            }

            DialogueHistorySlotView dialogueHistorySlotView = _spawnedSlotList[index];

            dialogueHistorySlotView.UpdateSlot(dialogueHistory[index]);
            dialogueHistorySlotView.SetActive(true);
        }

        for (int index = dialogueHistory.Count; index < _spawnedSlotList.Count; index++)
        {
            DialogueHistorySlotView deactivateDialogueHistorySlotView = _spawnedSlotList[index];

            if (deactivateDialogueHistorySlotView == null)
            {
                continue;
            }

            deactivateDialogueHistorySlotView.SetActive(false);
        }
    }

    private async UniTask RefreshScrollPosition()
    {
        await UniTask.Yield();

        Canvas.ForceUpdateCanvases();

        _scrollRect.verticalNormalizedPosition = 0f;
    }

}