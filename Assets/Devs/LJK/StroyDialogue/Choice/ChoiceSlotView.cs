using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceSlotView : MonoBehaviour
{
    [SerializeField] private Button _slotClickButton; 
    [SerializeField] private TMP_Text _choiceText;

    private string _nextDialogueId;

    public event Action<string> ChoiceSlotClicked;

    public void OnEnable()
    {
        _slotClickButton.onClick.AddListener(HandleSlotClicked);
    }

    public void OnDisable()
    {
        _slotClickButton.onClick.RemoveAllListeners();
    }

    public void Bind(ChoiceData choiceData)
    {
        _choiceText.text = choiceData.Text;
        _nextDialogueId = choiceData.NextDialogueId;
    }

    public void HandleSlotClicked()
    {
        if (ChoiceSlotClicked == null)
        {
            return;
        }

        ChoiceSlotClicked.Invoke(_nextDialogueId);
    }
}