using System;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceListView : MonoBehaviour
{
    [SerializeField] private List<ChoiceSlotView> _choiceSlotViews;

    public event Action<string> ChoiceSelected;

    private void OnEnable()
    {
        foreach (ChoiceSlotView choiceSlotView in _choiceSlotViews)
        {
            choiceSlotView.ChoiceSlotClicked += HandleChoiceSlotClicked;
        }
    }

    private void OnDisable()
    {
        foreach (ChoiceSlotView choiceSlotView in _choiceSlotViews)
        {
            choiceSlotView.ChoiceSlotClicked -= HandleChoiceSlotClicked;
        }
    }

    public void BindChoices(IReadOnlyList<ChoiceData> choiceDatas)
    {
        if (choiceDatas == null)
        {
            Debug.LogError($"[{nameof(ChoiceListView)}:{nameof(BindChoices)}] 전달된 ChoiceData 목록이 null입니다.");
            return;
        }

        for (int index = 0; index < _choiceSlotViews.Count; index++)
        {
            ChoiceSlotView choiceSlotView = _choiceSlotViews[index];

            bool isActive = index < choiceDatas.Count;
            choiceSlotView.gameObject.SetActive(isActive);

            if (isActive)
            {
                choiceSlotView.Bind(choiceDatas[index]);
            }
        }
    }

    private void HandleChoiceSlotClicked(string nextDialogueId)
    {
        if (ChoiceSelected == null)
        {
            return;
        }

        ChoiceSelected.Invoke(nextDialogueId);
    }
}