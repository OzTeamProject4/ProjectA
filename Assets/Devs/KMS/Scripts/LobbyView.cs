using UnityEngine;
using UnityEngine.UI;

public sealed class LobbyView : MonoBehaviour
{
    [SerializeField] private Button _profileButton;
    [SerializeField] private Button _giftBoxButton;
    [SerializeField] private Button _settingButton;
    [SerializeField] private Button _missionButton;
    [SerializeField] private Button _achievementButton;
    [SerializeField] private Button _dictionaryButton;
    [SerializeField] private Button _characterGachaButton;
    [SerializeField] private Button _inventoryButton;
    [SerializeField] private Button _characterButton;

    private void OnEnable()
    {
        Debug.Log("LobbyView Enabled");

        RegisterButtonEvents();
    }

    private void OnDisable()
    {
        UnregisterButtonEvents();
    }

    private void RegisterButtonEvents()
    {
        if (!ValidateButtons())
        {
            return;
        }

        _profileButton.onClick.AddListener(OnProfileButtonClicked);
        _giftBoxButton.onClick.AddListener(OnGiftBoxButtonClicked);
        _settingButton.onClick.AddListener(OnSettingButtonClicked);
        _missionButton.onClick.AddListener(OnMissionButtonClicked);
        _achievementButton.onClick.AddListener(OnAchievementButtonClicked);
        _dictionaryButton.onClick.AddListener(OnDictionaryButtonClicked);
        _characterGachaButton.onClick.AddListener(OnCharacterGachaButtonClicked);
        _inventoryButton.onClick.AddListener(OnInventoryButtonClicked);
        _characterButton.onClick.AddListener(OnCharacterButtonClicked);
    }

    private void UnregisterButtonEvents()
    {
        if (_profileButton != null)
        {
            _profileButton.onClick.RemoveListener(OnProfileButtonClicked);
        }

        if (_giftBoxButton != null)
        {
            _giftBoxButton.onClick.RemoveListener(OnGiftBoxButtonClicked);
        }

        if (_settingButton != null)
        {
            _settingButton.onClick.RemoveListener(OnSettingButtonClicked);
        }

        if (_missionButton != null)
        {
            _missionButton.onClick.RemoveListener(OnMissionButtonClicked);
        }

        if (_achievementButton != null)
        {
            _achievementButton.onClick.RemoveListener(OnAchievementButtonClicked);
        }

        if (_dictionaryButton != null)
        {
            _dictionaryButton.onClick.RemoveListener(OnDictionaryButtonClicked);
        }

        if (_characterGachaButton != null)
        {
            _characterGachaButton.onClick.RemoveListener(
                OnCharacterGachaButtonClicked);
        }

        if (_inventoryButton != null)
        {
            _inventoryButton.onClick.RemoveListener(OnInventoryButtonClicked);
        }

        if (_characterButton != null)
        {
            _characterButton.onClick.RemoveListener(OnCharacterButtonClicked);
        }
    }

    private bool ValidateButtons()
    {
        if (_profileButton == null)
        {
            Debug.LogError("[LobbyView] ProfileButton이 할당되지 않았습니다.");
            return false;
        }

        if (_giftBoxButton == null)
        {
            Debug.LogError("[LobbyView] GiftBoxButton이 할당되지 않았습니다.");
            return false;
        }

        if (_settingButton == null)
        {
            Debug.LogError("[LobbyView] SettingButton이 할당되지 않았습니다.");
            return false;
        }

        if (_missionButton == null)
        {
            Debug.LogError("[LobbyView] MissionButton이 할당되지 않았습니다.");
            return false;
        }

        if (_achievementButton == null)
        {
            Debug.LogError("[LobbyView] AchievementButton이 할당되지 않았습니다.");
            return false;
        }

        if (_dictionaryButton == null)
        {
            Debug.LogError("[LobbyView] DictionaryButton이 할당되지 않았습니다.");
            return false;
        }

        if (_characterGachaButton == null)
        {
            Debug.LogError("[LobbyView] CharacterGachaButton이 할당되지 않았습니다.");
            return false;
        }

        if (_inventoryButton == null)
        {
            Debug.LogError("[LobbyView] InventoryButton이 할당되지 않았습니다.");
            return false;
        }

        if (_characterButton == null)
        {
            Debug.LogError("[LobbyView] CharacterButton이 할당되지 않았습니다.");
            return false;
        }

        return true;
    }

    private void OnProfileButtonClicked()
    {
        Debug.Log("Profile Button Clicked");
    }

    private void OnGiftBoxButtonClicked()
    {
        Debug.Log("GiftBox Button Clicked");
    }

    private void OnSettingButtonClicked()
    {
        Debug.Log("Setting Button Clicked");
    }

    private void OnMissionButtonClicked()
    {
        Debug.Log("Mission Button Clicked");
    }

    private void OnAchievementButtonClicked()
    {
        Debug.Log("Achievement Button Clicked");
    }

    private void OnDictionaryButtonClicked()
    {
        Debug.Log("Dictionary Button Clicked");
    }

    private void OnCharacterGachaButtonClicked()
    {
        Debug.Log("CharacterGacha Button Clicked");
    }

    private void OnInventoryButtonClicked()
    {
        Debug.Log("Inventory Button Clicked");
    }

    private void OnCharacterButtonClicked()
    {
        Debug.Log("Character Button Clicked");
    }
}