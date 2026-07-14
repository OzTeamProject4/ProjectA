using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatItemView : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _statText;
    [SerializeField] private TMP_Text _bonusText;

    [SerializeField] private Sprite[] _statIcons;

    [Header("Bonus Stat Colors")]
    [SerializeField] private Color _increaseColor = Color.green;
    [SerializeField] private Color _decreaseColor = Color.red;
    [SerializeField] private Color _noChangeColor = Color.gray;

    public void SetDelta(StatDelta delta)
    {
        gameObject.SetActive(true);

        _statText.text = FormatValue(delta.Value, delta.IsInteger);

        _bonusText.text = BuildBonusText(delta);
        _bonusText.color = GetColor(delta.Delta);

        RefreshIcon(delta.Type);
        // TODO: 스텟별 아이콘 에셋 추가 시 _iconImage 에 스프라이트 설정
    }

    public void Hide()
    {
        gameObject.SetActive(false);
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

    private string BuildBonusText(StatDelta delta)
    {
        if (Mathf.Approximately(delta.Delta, 0f))
        {
            return "(±0)";
        }

        string sign = delta.Delta > 0f ? "+" : "-";
        float absDelta = Mathf.Abs(delta.Delta);

        return $"({sign}{FormatValue(absDelta, delta.IsInteger)})";
    }

    private string FormatValue(float value, bool isInteger)
    {
        if (isInteger)
        {
            return Mathf.RoundToInt(value).ToString();
        }

        return value.ToString("F2");
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

