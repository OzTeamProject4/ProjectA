using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleHUDView : BaseUI
{
    [Header("Top")]
    [SerializeField] private TMP_Text _stageText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private Slider _bossHpSlider;
    [SerializeField] private Image _settingICON;
    [SerializeField] private TMP_Text _missionText;

    [Header("Middle")]
    [SerializeField] private Image[] _characterImage;
    [SerializeField] private Slider[] _partyMemberHpSliders;
    [SerializeField] private Image[] _partyMemberGaugeImages;

    [Header("Bottom")]
    [SerializeField] private Slider _playerHpSlider;
    [SerializeField] private Slider _ultimateGaugeSlider;
    [SerializeField] private Slider _experienceSlider;
    [SerializeField] private Image _typeICON;
    [SerializeField] private Image _skillqImage;
    [SerializeField] private Image _skilleImage;
    [SerializeField] private Image _skillspaceImage;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private Image _switchCooldownOverlay;

    public void SetStage(string stageName)
    {
        _stageText.text = stageName;
    }

    public void SetTimer(float remainingSeconds)
    {
        int minutes = Mathf.FloorToInt(remainingSeconds / 60f);
        int seconds = Mathf.FloorToInt(remainingSeconds % 60f);
        _timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void SetBossHp(float currentHp, float maxHp)
    {
        if (maxHp <= 0f)
        {
            _bossHpSlider.value = 0f;
        }
        else
        {
            _bossHpSlider.value = currentHp / maxHp;
        }
    }

    public void SetPlayerHp(float currentHp, float maxHp)
    {
        if (_playerHpSlider == null)
        {
            return;
        }

        if (maxHp <= 0f)
        {
            _playerHpSlider.value = 0f;
        }
        else
        {
            _playerHpSlider.value = Mathf.Clamp01(currentHp / maxHp);
        }
    }

    public void SetUltimateGauge(float normalizedValue)
    {
        _ultimateGaugeSlider.value = Mathf.Clamp01(normalizedValue);

        if (_skillspaceImage != null)
        {
            _skillspaceImage.fillAmount = Mathf.Clamp01(normalizedValue);
        }
    }

    public void SetExperience(float normalizedValue)
    {
        _experienceSlider.value = Mathf.Clamp01(normalizedValue);
    }

    public void SetBasicSkillCooldown(float progress)
    {
        if (_skillqImage == null)
        {
            return;
        }

        _skillqImage.fillAmount = Mathf.Clamp01(1f - progress);
    }
    public void SetNormalSkillCooldown(float progress)
    {
        if (_skilleImage == null)
        {
            return;
        }

        _skilleImage.fillAmount = Mathf.Clamp01(1f - progress);
    }
    public void SetLevel(int level)
    {
        if (_levelText == null)
        {
            return;
        }
        _levelText.text = $"Lv.{level}";
    }
    public void SetPartyMemberHp(int index, float currentHp, float maxHp)
    {
        if (_partyMemberHpSliders == null)
        {
            return;
        }
        if (index < 0 || index >= _partyMemberHpSliders.Length)
        {
            return;
        }

        Slider slider = _partyMemberHpSliders[index];
        if (slider == null)
        {
            return;
        }

        if (maxHp <= 0f)
        {
            slider.value = 0f;
        }
        else
        {
            slider.value = Mathf.Clamp01(currentHp / maxHp);
        }
    }
    public void SetPartyMemberGauge(int index, float ratio)
    {
        if (_partyMemberGaugeImages == null)
        {
            return;
        }
        if (index < 0 || index >= _partyMemberGaugeImages.Length)
        {
            return;
        }

        Image gaugeImage = _partyMemberGaugeImages[index];
        if (gaugeImage == null)
        {
            return;
        }

        gaugeImage.fillAmount = Mathf.Clamp01(ratio);
    }
    public void SetSwitchCooldown(float progress)
    {
        if (_switchCooldownOverlay == null)
        {
            return;
        }
        _switchCooldownOverlay.fillAmount = Mathf.Clamp01(1f - progress);
    }
}
