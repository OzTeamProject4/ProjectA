using UnityEngine;
using UnityEngine.UI;

public class DictionaryAboutBackButtonView : MonoBehaviour
{
    [SerializeField] private GameObject _screenRoot;
    [SerializeField] private GameObject _aboutScreen;

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
        if (_aboutScreen == null)
        {
            Debug.LogError("DictionaryScreen is not assigned.");
            return;
        }

        _aboutScreen.SetActive(false);

        if (_screenRoot != null)
        {
            _screenRoot.SetActive(false);
        }

        Debug.Log("Back To Lobby From Dictionary");
    }
}
