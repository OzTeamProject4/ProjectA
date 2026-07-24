using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlotView : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _countText;

    public void Bind(RewardSlotViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("[RewardSlotView] Bind: viewModel 이 null 입니다.");
            return;
        }

        if (null != _countText)
        {
            _countText.text = viewModel.Count.ToString();
        }

        LoadIconAsync(viewModel.GetIconPath()).Forget();
    }

    private async UniTaskVoid LoadIconAsync(string iconPath)
    {
        if (null == _iconImage)
        {
            return;
        }

        if (string.IsNullOrEmpty(iconPath))
        {
            _iconImage.enabled = false;
            return;
        }

        Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(iconPath, destroyCancellationToken);

        if (null == sprite)
        {
            _iconImage.enabled = false;
            return;
        }

        _iconImage.sprite = sprite;
        _iconImage.enabled = true;
    }
}
