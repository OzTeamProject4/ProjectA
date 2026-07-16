using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainDungeonButtonView : MonoBehaviour
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
        if (_screenRoot == null)
        {
            Debug.LogError("ScreenRoot is not assigned.");
            return;
        }

        if (_stageSelectScreen == null)
        {
            Debug.LogError("StageSelectScreen is not assigned.");
            return;
        }

        _screenRoot.SetActive(true);
        _stageSelectScreen.SetActive(true);

        Debug.Log("Stage Select Screen Opened");
    }
}
