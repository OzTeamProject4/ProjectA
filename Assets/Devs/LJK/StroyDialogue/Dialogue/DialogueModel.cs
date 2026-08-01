using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class DialogueModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs BackgroundChanged = new PropertyChangedEventArgs(nameof(Background));
    private static readonly PropertyChangedEventArgs SpeakerNameTextChanged = new PropertyChangedEventArgs(nameof(SpeakerNameText));
    private static readonly PropertyChangedEventArgs DialogueTextChanged = new PropertyChangedEventArgs(nameof(DialogueText));
    private static readonly PropertyChangedEventArgs LeftCharacterIdChanged = new PropertyChangedEventArgs(nameof(LeftCharacterId));
    private static readonly PropertyChangedEventArgs CenterCharacterIdChanged = new PropertyChangedEventArgs(nameof(CenterCharacterId));
    private static readonly PropertyChangedEventArgs RightCharacterIdChanged = new PropertyChangedEventArgs(nameof(RightCharacterId));
    private static readonly PropertyChangedEventArgs ActiveCharacterIdChanged = new PropertyChangedEventArgs(nameof(ActiveCharacterId));
    private static readonly PropertyChangedEventArgs IsChoiceOpenChanged = new PropertyChangedEventArgs(nameof(IsChoiceOpen));
    private static readonly PropertyChangedEventArgs ChoicesChanged = new PropertyChangedEventArgs(nameof(Choices));
    private static readonly PropertyChangedEventArgs AutoModeChanged = new PropertyChangedEventArgs(nameof(IsAutoMode));

    private string _background;
    private string _speakerNameText;
    private string _dialogueText;
    private bool _isChoiceOpen;

    private string _leftCharacterId;
    private string _centerCharacterId;
    private string _rightCharacterId;

    public string _activeCharacterId;

    private bool _isAutoMode;

    private IReadOnlyList<ChoiceData> _choices = Array.Empty<ChoiceData>();

    private readonly List<DialogueData> _dialogueHistory = new List<DialogueData>();

    public string Background
    {
        get { return _background; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            if (_background != value)
            {
                _background = value;
                OnPropertyChanged(BackgroundChanged);
            }
        }
    }

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

    public string LeftCharacterId
    {
        get { return _leftCharacterId; }
        private set
        {
            if (_leftCharacterId != value)
            {
                _leftCharacterId = value;
                OnPropertyChanged(LeftCharacterIdChanged);
            }
        }
    }

    public string CenterCharacterId
    {
        get { return _centerCharacterId; }
        private set
        {
            if (_centerCharacterId != value)
            {
                _centerCharacterId = value;
                OnPropertyChanged(CenterCharacterIdChanged);
            }
        }
    }

    public string RightCharacterId
    {
        get { return _rightCharacterId; }
        private set
        {
            if (_rightCharacterId != value)
            {
                _rightCharacterId = value;
                OnPropertyChanged(RightCharacterIdChanged);
            }
        }
    }

    public string ActiveCharacterId
    {
        get { return _activeCharacterId; }
        private set
        {
            if (_activeCharacterId != value)
            {
                _activeCharacterId = value;
                OnPropertyChanged(ActiveCharacterIdChanged);
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

    public bool IsAutoMode
    {
        get { return _isAutoMode; }
        private set
        {
            if (_isAutoMode != value)
            {
                _isAutoMode = value;
                OnPropertyChanged(AutoModeChanged);
            }
        }
    }

    public IReadOnlyList<ChoiceData> Choices
    {
        get { return _choices; }
        set
        {
            if (_choices != value)
            {
                _choices = value;
                OnPropertyChanged(ChoicesChanged);
            }
        }
    }

    public IReadOnlyList<DialogueData> DialogueHistory
    {
        get 
        {
            return _dialogueHistory; 
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void UpdateDialogue(DialogueData dialogueData)
    {
        _dialogueHistory.Add(dialogueData);

        if (dialogueData == null)
        {
            Debug.LogError($"[{nameof(DialogueModel)}:{nameof(UpdateDialogue)}] 전달된 DialogueData가 null입니다.");
            return;
        }

        Background = dialogueData.Background;
        SpeakerNameText = dialogueData.SpeakerName;
        DialogueText = dialogueData.Text;
        LeftCharacterId = dialogueData.LeftCharacter;
        CenterCharacterId = dialogueData.CenterCharacter;
        RightCharacterId = dialogueData.RightCharacter;
        ActiveCharacterId = dialogueData.ActiveCharacter;
    }

    public void Initialize()
    {
        SetAutoMode(false);
        Choices = Array.Empty<ChoiceData>();
        _dialogueHistory.Clear();
    }

    public void SetChoices(IReadOnlyList<ChoiceData> choiceDatas)
    {
        Choices = choiceDatas;
    }

    public void SetChoiceOpen(bool isOpen)
    {
        IsChoiceOpen = isOpen;
    }

    public void SetAutoMode(bool isAutoMode)
    {
        IsAutoMode = isAutoMode;
    }

    public void NotifyAllProperties()
    {
        OnPropertyChanged(BackgroundChanged);
        OnPropertyChanged(SpeakerNameTextChanged);
        OnPropertyChanged(DialogueTextChanged);
        OnPropertyChanged(LeftCharacterIdChanged);
        OnPropertyChanged(CenterCharacterIdChanged);
        OnPropertyChanged(RightCharacterIdChanged);
        OnPropertyChanged(IsChoiceOpenChanged);
        OnPropertyChanged(ChoicesChanged);
        OnPropertyChanged(ActiveCharacterIdChanged);
        OnPropertyChanged(AutoModeChanged);
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