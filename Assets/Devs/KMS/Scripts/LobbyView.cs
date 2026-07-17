using UnityEngine.UI;
using UnityEngine;

public class LobbyView : MonoBehaviour
{
    [SerializeField] private Button _profileButton;
    [SerializeField] private Button _giftBoxButton;
    [SerializeField] private Button _settingButton;
    [SerializeField] private Button _missionButton;
    [SerializeField] private Button _achievementButton;

    [SerializeField] private Button _dictionaryButton;
    [SerializeField] private Button _characterGachaButton;
    [SerializeField] private Button _inventoryButton;
    [SerializeField] private Button _farmingDungeonButton;
    [SerializeField] private Button _mainDungeonButton;
    [SerializeField] private Button _characterButton;


    private void OnEnable()
    {
        Debug.Log("LobbyView Enabled");
        RegisterButtonEvent();
    }

    private void OnDisable()
    {
        UnRegisterButtonEvents();
    }

    private void RegisterButtonEvent()
    {
        _profileButton.onClick.AddListener(OnProfileButtonClicked);
        _giftBoxButton.onClick.AddListener(OnGiftBoxButtonClicked);
        _settingButton.onClick.AddListener(OnSettingButtonClicked);
        _missionButton.onClick.AddListener(OnMissionButtonClicked);
        _achievementButton.onClick.AddListener(OnAchievementButtonClicked);

        _dictionaryButton.onClick.AddListener(OnDictionaryButtonClicked);
        _characterGachaButton.onClick.AddListener(OnCharacterGachaButtonClicked);
        _inventoryButton.onClick.AddListener(OnInventoryButtonClicked);
        _farmingDungeonButton.onClick.AddListener(OnFarmingDungeonButtonClicked);
        _mainDungeonButton.onClick.AddListener(OnmainDungeonButtonClicked);
        _characterButton.onClick.AddListener(OncharacterButtonClicked);

    }

    private void UnRegisterButtonEvents()
    {
        _profileButton.onClick.RemoveListener(OnProfileButtonClicked);
        _giftBoxButton.onClick.RemoveListener(OnGiftBoxButtonClicked);
        _settingButton.onClick.RemoveListener(OnSettingButtonClicked);
        _missionButton.onClick.RemoveListener(OnMissionButtonClicked);
        _achievementButton.onClick.RemoveListener(OnAchievementButtonClicked);

        _dictionaryButton.onClick.RemoveListener(OnDictionaryButtonClicked);
        _characterGachaButton.onClick.RemoveListener(OnCharacterGachaButtonClicked);
        _inventoryButton.onClick.RemoveListener(OnInventoryButtonClicked);
        _farmingDungeonButton.onClick.RemoveListener(OnFarmingDungeonButtonClicked);
        _mainDungeonButton.onClick.RemoveListener(OnmainDungeonButtonClicked);
        _characterButton.onClick.RemoveListener(OncharacterButtonClicked);

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

    private void OnFarmingDungeonButtonClicked()
    {
        Debug.Log("FarmingDungeon Button Clicked");
    }

    private void OnmainDungeonButtonClicked()
    {
        Debug.Log("mainDungeon Button Clicked");
    }

    private void OncharacterButtonClicked()
    {
        Debug.Log("characterButton Button Clicked");
    }
}
