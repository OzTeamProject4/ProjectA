using System;
using System.ComponentModel;

public class DialogueViewModel
{
    private DialogueModel _dialogueModel;

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
