using UnityEngine;
using UnityEngine.UI;

public class CharacterGachaScreenView : MonoBehaviour
{
    private const int OneDrawCount = 1;
    private const int TenDrawCount = 10;

    [SerializeField] private GameObject _screenRoot;

    [SerializeField] private GameObject _characterGachaMainScreen;
    [SerializeField] private GameObject _characterGachaSelectScreen;
    [SerializeField] private GameObject _characterGachaResultScreen;

    [SerializeField] private Button _backToLobbyButton;
    [SerializeField] private Button _openDrawSelectButton;

    [SerializeField] private Button _backToMainButton;
    [SerializeField] private Button _drawTenButton;
    [SerializeField] private Button _drawOneButton;

    [SerializeField] private Button _drawAgainButton;
    [SerializeField] private Button _confirmButton;

    private int _lastDrawCount;
    private bool _isButtonEventRegistered;

    private void OnEnable()
    {
        if (HasMissingReference())
        {
            return;
        }

        RegisterButtonEvents();

        _lastDrawCount = 0;

        ShowMainScreen();
    }

    private void OnDisable()
    {
        UnRegisterButtonEvents();
    }

    private void RegisterButtonEvents()
    {
        if (_isButtonEventRegistered)
        {
            return;
        }

        _backToLobbyButton.onClick.AddListener(OnBackToLobbyButtonClicked);
        _openDrawSelectButton.onClick.AddListener(OnOpenDrawSelectButtonClicked);

        _backToMainButton.onClick.AddListener(OnBackToMainButtonClicked);
        _drawTenButton.onClick.AddListener(OnDrawTenButtonClicked);
        _drawOneButton.onClick.AddListener(OnDrawOneButtonClicked);

        _drawAgainButton.onClick.AddListener(OnDrawAgainButtonClicked);
        _confirmButton.onClick.AddListener(OnConfirmButtonClicked);

        _isButtonEventRegistered = true;
    }

    private void UnRegisterButtonEvents()
    {
        if (!_isButtonEventRegistered)
        {
            return;
        }

        _backToLobbyButton.onClick.RemoveListener(OnBackToLobbyButtonClicked);
        _openDrawSelectButton.onClick.RemoveListener(OnOpenDrawSelectButtonClicked);

        _backToMainButton.onClick.RemoveListener(OnBackToMainButtonClicked);
        _drawTenButton.onClick.RemoveListener(OnDrawTenButtonClicked);
        _drawOneButton.onClick.RemoveListener(OnDrawOneButtonClicked);

        _drawAgainButton.onClick.RemoveListener(OnDrawAgainButtonClicked);
        _confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);

        _isButtonEventRegistered = false;
    }

    private void OnBackToLobbyButtonClicked()
    {
        gameObject.SetActive(false);
        _screenRoot.SetActive(false);

        Debug.Log("Returned To Lobby.");
    }

    private void OnOpenDrawSelectButtonClicked()
    {
        ShowSelectScreen();

        Debug.Log("Gacha Draw Select Screen Opened.");
    }

    private void OnBackToMainButtonClicked()
    {
        ShowMainScreen();

        Debug.Log("Returned To Character Gacha Main Screen.");
    }

    private void OnDrawTenButtonClicked()
    {
        ExecuteDraw(TenDrawCount);
    }

    private void OnDrawOneButtonClicked()
    {
        ExecuteDraw(OneDrawCount);
    }

    private void OnDrawAgainButtonClicked()
    {
        if (_lastDrawCount <= 0)
        {
            Debug.LogWarning("Previous draw count does not exist.");
            return;
        }

        ExecuteDraw(_lastDrawCount);
    }

    private void OnConfirmButtonClicked()
    {
        _lastDrawCount = 0;

        ShowMainScreen();

        Debug.Log("Gacha Result Confirmed.");
    }

    private void ExecuteDraw(int drawCount)
    {
        _lastDrawCount = drawCount;

        ShowResultScreen();

        Debug.Log($"{drawCount} Character Draw Executed.");
    }

    private void ShowMainScreen()
    {
        _characterGachaMainScreen.SetActive(true);
        _characterGachaSelectScreen.SetActive(false);
        _characterGachaResultScreen.SetActive(false);
    }

    private void ShowSelectScreen()
    {
        _characterGachaMainScreen.SetActive(false);
        _characterGachaSelectScreen.SetActive(true);
        _characterGachaResultScreen.SetActive(false);
    }

    private void ShowResultScreen()
    {
        _characterGachaMainScreen.SetActive(false);
        _characterGachaSelectScreen.SetActive(false);
        _characterGachaResultScreen.SetActive(true);
    }

    private bool HasMissingReference()
    {
        if (_screenRoot == null)
        {
            Debug.LogError("ScreenRoot is not assigned.");
            return true;
        }

        if (_characterGachaMainScreen == null)
        {
            Debug.LogError("CharacterGachaMainScreen is not assigned.");
            return true;
        }

        if (_characterGachaSelectScreen == null)
        {
            Debug.LogError("CharacterGachaSelectScreen is not assigned.");
            return true;
        }

        if (_characterGachaResultScreen == null)
        {
            Debug.LogError("CharacterGachaResultScreen is not assigned.");
            return true;
        }

        if (_backToLobbyButton == null ||
            _openDrawSelectButton == null ||
            _backToMainButton == null ||
            _drawTenButton == null ||
            _drawOneButton == null ||
            _drawAgainButton == null ||
            _confirmButton == null)
        {
            Debug.LogError("Character Gacha Button reference is not assigned.");
            return true;
        }

        return false;
    }
}
