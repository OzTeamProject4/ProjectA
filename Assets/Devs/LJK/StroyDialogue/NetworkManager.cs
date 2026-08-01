using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private DialogueModel _dialogueModel;

    public DialogueModel DialogueModel
    {
        get
        {
            if (_dialogueModel == null)
            {
                _dialogueModel = new DialogueModel();
            }

            return _dialogueModel;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
