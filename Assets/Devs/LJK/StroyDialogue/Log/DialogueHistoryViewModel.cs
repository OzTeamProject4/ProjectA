using System.Collections.Generic;

public class DialogueHistoryViewModel
{
    private DialogueModel _dialogueModel;

    public IReadOnlyList<DialogueData> DialogueHistory
    {
        get
        {
            return _dialogueModel.DialogueHistory;
        }
    }

    public DialogueHistoryViewModel()
    {
        _dialogueModel = NetworkManager.Instance.DialogueModel;
    }

    public void Dispose()
    {
        _dialogueModel = null;
    }
}
