using System.ComponentModel;
using UnityEngine;

public class DialogueModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs SpeakerNameTextChanged = new PropertyChangedEventArgs(nameof(SpeakerNameText));
    private static readonly PropertyChangedEventArgs DialogueTextChanged = new PropertyChangedEventArgs(nameof(DialogueText));
    private static readonly PropertyChangedEventArgs IsChoiceOpenChanged = new PropertyChangedEventArgs(nameof(IsChoiceOpen));

    private string _speakerNameText;
    private string _dialogueText;
    private bool _isChoiceOpen;

    public string SpeakerNameText
    {
        get { return _speakerNameText; }
        private set
        {
            if (_speakerNameText != value)
            {
                _speakerNameText = value;
                OnPropertyChanged(SpeakerNameTextChanged);
            }
        }
    }

    public string DialogueText
    {
        get { return _dialogueText; }
        private set
        {
            if (_dialogueText != value)
            {
                _dialogueText = value;
                OnPropertyChanged(DialogueTextChanged);
            }
        }
    }

    public bool IsChoiceOpen
    {
        get { return _isChoiceOpen; }
        private set
        {
            if (_isChoiceOpen != value)
            {
                _isChoiceOpen = value;
                OnPropertyChanged(IsChoiceOpenChanged);
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void UpdateDialogue(DialogueData dialogueData)
    {
        if (dialogueData == null)
        {
            Debug.LogError($"[{nameof(DialogueModel)}:{nameof(UpdateDialogue)}] 전달된 DialogueData가 null입니다.");
            return;
        }

        SpeakerNameText = dialogueData.SpeakerName;
        DialogueText = dialogueData.Text;
    }

    public void SetChoiceOpen(bool isOpen)
    {
        if (_isChoiceOpen == isOpen)
        {
            return;
        }

        _isChoiceOpen = isOpen;
    }

    public void NotifyAllProperties()
    {
        OnPropertyChanged(SpeakerNameTextChanged);
        OnPropertyChanged(DialogueTextChanged);
        OnPropertyChanged(IsChoiceOpenChanged);
    }

    private void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(this, propertyChangedEventArgs);
    }
}