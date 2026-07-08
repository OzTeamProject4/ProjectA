using UnityEngine;
using UnityEngine.UI;
public class DictionaryButtonView : MonoBehaviour
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
        if (_screenRoot == null)
        {
            Debug.LogError("ScreenRoot is not assigned.");
            return;
        }

        if (_dictionaryScreen == null)
        {
            Debug.LogError("DictionaryScreen is not assigned.");
            return;
        }

        _screenRoot.SetActive(true);
        _dictionaryScreen.SetActive(true);

        Debug.Log("Dictionary Screen Opened");
    }
}
