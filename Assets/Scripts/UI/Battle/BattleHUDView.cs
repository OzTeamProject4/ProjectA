using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleHUDView : BaseUI
{
    private static readonly Color AlivePortraitColor = Color.white;
    private static readonly Color DeadPortraitColor = new Color(0.4f, 0.4f, 0.4f, 0.6f);

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
    [SerializeField] private GameObject[] _partyMemberSlots;
    [SerializeField] private TMP_Text[] _partyMemberNumberTexts;
    [SerializeField] private Image[] _partyMemberElementImages;

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
    [SerializeField] private TMP_Text _playerHpText;
    [SerializeField] private TMP_Text _ultimateGaugeText;
    [SerializeField] private Image _basicSkillIconImage;
    [SerializeField] private Image _normalSkillIconImage;
    [SerializeField] private Image _ultimateSkillIconImage;

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

        if (_playerHpText != null)
        {
            _playerHpText.text = $"HP : {currentHp:0}/{maxHp:0}";
        }
    }
    public void SetUltimateGauge(int current, int max)
    {
        float ratio = 0f;
        if (max > 0)
        {
            ratio = (float)current / max;
        }
        _ultimateGaugeSlider.value = Mathf.Clamp01(ratio);
        if (_skillspaceImage != null)
        {
            _skillspaceImage.fillAmount = Mathf.Clamp01(1f - ratio);
        }
        if (_ultimateGaugeText != null)
        {
            _ultimateGaugeText.text = $"{current}/{max}";
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
    public void SetBasicSkillIcon(Sprite icon)
    {
        if (_basicSkillIconImage == null)
        {
            return;
        }
        _basicSkillIconImage.sprite = icon;
    }
    public void SetNormalSkillIcon(Sprite icon)
    {
        if (_normalSkillIconImage == null)
        {
            return;
        }
        _normalSkillIconImage.sprite = icon;
    }
    public void SetUltimateSkillIcon(Sprite icon)
    {
        if (_ultimateSkillIconImage == null)
        {
            return;
        }
        _ultimateSkillIconImage.sprite = icon;
    }
    public void SetPartyMemberPortrait(int index, Sprite portrait)
    {
        if (_characterImage == null)
        {
            return;
        }
        if (index < 0 || index >= _characterImage.Length)
        {
            return;
        }
        Image image = _characterImage[index];
        if (image == null)
        {
            return;
        }
        image.sprite = portrait;
    }
    public void SetPartyMemberDead(int index, bool isDead)
    {
        if (_characterImage == null)
        {
            return;
        }

        if (index < 0 || index >= _characterImage.Length)
        {
            return;
        }

        Image image = _characterImage[index];

        if (image == null)
        {
            return;
        }

        if (isDead)
        {
            image.color = DeadPortraitColor;
            return;
        }

        image.color = AlivePortraitColor;
    }

    public void SetCharacterIcon(Sprite icon)
    {
        if (_typeICON == null)
        {
            return;
        }
        _typeICON.sprite = icon;
    }
    public void SetPartyMemberCount(int count)
    {
        if (_partyMemberSlots == null)
        {
            return;
        }
        for (int i = 0; i < _partyMemberSlots.Length; i++)
        {
            GameObject slot = _partyMemberSlots[i];
            if (slot == null)
            {
                continue;
            }
            slot.SetActive(i < count);
        }
    }
    public void SetPartyMemberNumber(int index, int number)
    {
        if (_partyMemberNumberTexts == null) { return; }
        if (index < 0 || index >= _partyMemberNumberTexts.Length) { return; }
        TMP_Text text = _partyMemberNumberTexts[index];
        if (text == null) { return; }
        text.text = number.ToString();
    }
    public void SetPartyMemberElement(int index, Sprite element)
    {
        if (_partyMemberElementImages == null)
        {
            return;
        }
        if (index < 0 || index >= _partyMemberElementImages.Length)
        {
            return;
        }
        Image image = _partyMemberElementImages[index];
        if (image == null)
        {
            return;
        }
        image.sprite = element;
    }
}
