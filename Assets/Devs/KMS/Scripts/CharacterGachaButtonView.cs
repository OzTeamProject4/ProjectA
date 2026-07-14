using UnityEngine;
using UnityEngine.UI;


public class CharacterGachaButtonView : MonoBehaviour
{
    [SerializeField] private GameObject _screenRoot;
    [SerializeField] private GameObject _characterGachaScreen;

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

        if (_characterGachaScreen == null)
        {
            Debug.LogError("CharacterGachaScreen is not assigned.");
            return;
        }

        _characterGachaScreen.SetActive(true);
        _screenRoot.SetActive(true);

        Debug.Log("Character Gacha Screen Opened.");
    }
}
