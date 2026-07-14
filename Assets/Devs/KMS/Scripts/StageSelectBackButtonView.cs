using UnityEngine;
using UnityEngine.UI;



public class StageSelectBackButtonView : MonoBehaviour
{
    [SerializeField] private GameObject _screenRoot;
    [SerializeField] private GameObject _stageSelectScreen;

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
        if (_stageSelectScreen == null)
        {
            Debug.LogError("StageSelectScreen is not assigned.");
            return;
        }

        _stageSelectScreen.SetActive(false);

        if (_screenRoot != null)
        {
            _screenRoot.SetActive(false);
        }

        Debug.Log("Back To Lobby");
    }
}
