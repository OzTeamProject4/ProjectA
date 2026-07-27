using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentStatOptionView : MonoBehaviour
{
    private const string StatValueFormat = "0.##";

    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _statText;
    [SerializeField] private TMP_Text _bonusText;

    [SerializeField] private Sprite[] _statIcons;

    [Header("Bonus Stat Colors")]
    [SerializeField] private Color _increaseColor = Color.green;
    [SerializeField] private Color _decreaseColor = Color.red;
    [SerializeField] private Color _noChangeColor = Color.gray;

    public void SetStat(StatDelta statDelta)
    {
        _statText.text = FormatValue(statDelta.Value);

        RefreshBonusText(statDelta);
        RefreshIcon(statDelta.Type);
    }

    private void RefreshBonusText(StatDelta statDelta)
    {
        if (null == _bonusText)
        {
            return;
        }

        if (!statDelta.HasComparison)
        {
            _bonusText.gameObject.SetActive(false);
            return;
        }

        _bonusText.gameObject.SetActive(true);
        _bonusText.text = BuildBonusText(statDelta);
        _bonusText.color = GetColor(statDelta.Delta);
    }

    private void RefreshIcon(StatType type)
    {
        if (null == _iconImage)
        {
            return;
        }

        int index = (int)type;

        if (null == _statIcons || index >= _statIcons.Length || null == _statIcons[index])
        {
            _iconImage.enabled = false;
            return;
        }

        _iconImage.enabled = true;
        _iconImage.sprite = _statIcons[index];
    }

    private string BuildBonusText(StatDelta statDelta)
    {
        if (Mathf.Approximately(statDelta.Delta, 0f))
        {
            return "(±0)";
        }

        string sign = statDelta.Delta > 0f ? "+" : "-";
        float absDelta = Mathf.Abs(statDelta.Delta);

        return $"({sign}{FormatValue(absDelta)})";
    }

    private string FormatValue(float value)
    {
        return value.ToString(StatValueFormat);
    }

    private Color GetColor(float delta)
    {
        if (Mathf.Approximately(delta, 0f))
        {
            return _noChangeColor;
        }

        if (delta > 0f)
        {
            return _increaseColor;
        }

        return _decreaseColor;
    }
}
