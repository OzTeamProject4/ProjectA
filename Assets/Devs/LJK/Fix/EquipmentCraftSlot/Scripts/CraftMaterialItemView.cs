using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftMaterialItemView : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _tierText;
    [SerializeField] private TMP_Text _countText;

    private void Awake()
    {
        UnityUtil.ValidateReference(_iconImage, nameof(CraftMaterialItemView), nameof(_iconImage));
        UnityUtil.ValidateReference(_countText, nameof(CraftMaterialItemView), nameof(_countText));
    }

    public void UpdateView(string tier, int ownedCount, int requiredCount)
    {
        if (_tierText != null)
        {
            _tierText.text = string.IsNullOrWhiteSpace(tier) ? "-" : tier;
        }

        _countText.text = $"{ownedCount} / {requiredCount}";
    }

    public UniTask UpdateIconAsync(string iconKey, CancellationToken cancellationToken)
    {
        return SpriteLoader.LoadIntoAsync(_iconImage, iconKey, cancellationToken);
    }
}
