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

    public async UniTask UpdateIconAsync(string iconKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(iconKey))
        {
            _iconImage.enabled = false;
            return;
        }

        Sprite iconSprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(iconKey, cancellationToken);

        if (iconSprite == null)
        {
            _iconImage.enabled = false;
            return;
        }

        _iconImage.enabled = true;
        _iconImage.sprite = iconSprite;
    }
}
