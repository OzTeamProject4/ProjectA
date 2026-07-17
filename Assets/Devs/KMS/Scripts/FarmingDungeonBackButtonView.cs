
using UnityEngine;
using UnityEngine.UI;

public class FarmingDungeonBackButtonView : MonoBehaviour
{
    [SerializeField] private GameObject _screenRoot;
    [SerializeField] private GameObject _farmingDungeonScreen;

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

        if (_farmingDungeonScreen == null)
        {
            Debug.LogError("FarmingDungeonScreen is not assigned.");
            return;
        }

        _farmingDungeonScreen.SetActive(false);
        _screenRoot.SetActive(false);

        Debug.Log("Returned To Lobby.");
    }
}
