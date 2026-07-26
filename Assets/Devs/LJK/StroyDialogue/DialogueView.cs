using TMPro;
using UnityEngine;

public class DialogueView : BaseUI
{
    [Header("Dialogue")]
    [SerializeField] private TMP_Text _speakerNameText;
    [SerializeField] private TMP_Text _dialogueText;

    [Header("Buttons")]
    [SerializeField] private DialogueButton _nextButton;

    private DialogueViewModel _dialogueViewModel;

    private void Awake()
    {
        UnityUtil.ValidateReference(_speakerNameText, nameof(AudioClip), nameof(_speakerNameText));
        UnityUtil.ValidateReference(_dialogueText, nameof(AudioClip), nameof(_dialogueText));

        BindAllDialogueButton();

        _dialogueViewModel = new DialogueViewModel();
    }

    private void OnEnable()
    {
        _dialogueViewModel.OnEnable();
        _dialogueViewModel.ModelPropertyChanged += OnModelPropertyChanged;

        _dialogueViewModel.Refresh();
    }

    private void OnDisable()
    {
        _dialogueViewModel.OnDisable();
        _dialogueViewModel.ModelPropertyChanged -= OnModelPropertyChanged;
    }

    private void OnDestroy()
    {
        _dialogueViewModel.Dispose();
        _dialogueViewModel = null;
    }

    private void OnModelPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_dialogueViewModel.SpeakerNameText):
                UpdateSpeakerNameText();
                break;
            case nameof(_dialogueViewModel.DialogueText):
                UpdateSpeakerDialogueText();
                break;
        }
    }

    private void UpdateSpeakerNameText()
    {
        _speakerNameText.text = _dialogueViewModel.SpeakerNameText;
    }

    private void UpdateSpeakerDialogueText()
    {
        _dialogueText.text = _dialogueViewModel.DialogueText;
    }

    private void BindAllDialogueButton()
    {
        _nextButton.ButtonClicked += HandleNextClicked;
    }

    private void HandleNextClicked()
    {
        GameManager.Instance.DialogueManager.AdvanceDialogue();
    }
}
