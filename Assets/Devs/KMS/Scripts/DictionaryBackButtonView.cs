using UnityEngine;
using UnityEngine.UI;

public class DictionaryBackButtonView : MonoBehaviour
{
    [SerializeField] private GameObject _screenRoot;
    [SerializeField] private GameObject _dictionaryScreen;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (_dictionaryScreen == null)
        {
            Debug.LogError("DictionaryScreen is not assigned.");
            return;
        }

        _dictionaryScreen.SetActive(false);

        if (_screenRoot != null)
        {
            _screenRoot.SetActive(false);
        }

        Debug.Log("Back To Lobby From Dictionary");
    }
}
