using System;
using System.Collections.Generic;
using System.ComponentModel;

public class DialogueViewModel
{
    private DialogueModel _dialogueModel;

    public string Background
    {
        get 
        {
            return _dialogueModel.Background; 
        }
    }

    public string SpeakerNameText
    {
        get 
        {
            return _dialogueModel.SpeakerNameText; 
        }
    }

    public string DialogueText
    {
        get 
        {
            return _dialogueModel.DialogueText;
        }
    }

    public string LeftCharacterId
    {
        get 
        {
            return _dialogueModel.LeftCharacterId;
        }
    }

    public string CenterCharacterId
    {
        get 
        { 
            return _dialogueModel.CenterCharacterId; 
        }
    }

    public string RightCharacterId
    {
        get 
        { 
            return _dialogueModel.RightCharacterId; 
        }
    }

    public string ActiveCharacterId
    {
        get 
        { 
            return _dialogueModel.ActiveCharacterId; 
        }
    }

    public IReadOnlyList<ChoiceData> Choices
    {
        get 
        {
            return _dialogueModel.Choices;
        }
    }

    public bool IsChoiceOpen
    {
        get 
        {
            return _dialogueModel.IsChoiceOpen;
        }
    }

    public bool IsAutoMode
    {
        get 
        {
            return _dialogueModel.IsAutoMode;
        }
    }

    public event Action<string> ModelPropertyChanged;

    public DialogueViewModel()
    {
        _dialogueModel = NetworkManager.Instance.DialogueModel;
    }

    public void OnEnable()
    {
        _dialogueModel.PropertyChanged += OnModelPropertyChanged;
    }

    public void OnDisable()
    {
        _dialogueModel.PropertyChanged -= OnModelPropertyChanged;
    }

    public void Dispose()
    {
        _dialogueModel = null;
    }

    public void Refresh()
    {
        _dialogueModel.NotifyAllProperties();
    }

    private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (ModelPropertyChanged == null)
        {
            return;
        }

        ModelPropertyChanged.Invoke(e.PropertyName);
    }
}
