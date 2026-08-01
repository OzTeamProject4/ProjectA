using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private DialogueModel _dialogueModel;

    private StudentGachaModel _studentGachaModel;

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

    public StudentGachaModel StudentGachaModel
    {
        get
        {
            if (_studentGachaModel == null)
            {
                _studentGachaModel = new StudentGachaModel();
            }

            return _studentGachaModel;
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
