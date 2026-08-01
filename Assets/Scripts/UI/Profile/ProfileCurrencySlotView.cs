using Cysharp.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileCurrencySlotView : MonoBehaviour
{
    private const string CountFormat = "N0";

    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _countText;

    private MaterialModel _materialModel;
    private CancellationTokenSource _iconCts;

    private void Awake()
    {
        UnityUtil.ValidateReference(_iconImage, nameof(ProfileCurrencySlotView), nameof(_iconImage));
        UnityUtil.ValidateReference(_nameText, nameof(ProfileCurrencySlotView), nameof(_nameText));
        UnityUtil.ValidateReference(_countText, nameof(ProfileCurrencySlotView), nameof(_countText));
    }

    private void OnDisable()
    {
        ClearModel();
    }

    public void SetModel(MaterialModel materialModel)
    {
        ClearModel();

        _materialModel = materialModel;

        if (_materialModel == null)
        {
            Debug.LogWarning($"[{nameof(ProfileCurrencySlotView)}:{nameof(SetModel)}] MaterialModel이 null이라 슬롯을 갱신할 수 없습니다.");
            return;
        }

        _materialModel.PropertyChanged += OnMaterialModelChanged;

        UpdateNameText();
        UpdateCountText();

        _iconCts = new CancellationTokenSource();
        UpdateIconAsync(_iconCts.Token).Forget();
    }

    private void ClearModel()
    {
        if (_iconCts != null)
        {
            _iconCts.Cancel();
            _iconCts.Dispose();
            _iconCts = null;
        }

        if (_materialModel != null)
        {
            _materialModel.PropertyChanged -= OnMaterialModelChanged;
            _materialModel = null;
        }
    }

    private void OnMaterialModelChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(MaterialModel.Count))
        {
            return;
        }

        UpdateCountText();
    }

    private void UpdateNameText()
    {
        _nameText.text = _materialModel.Name;
    }

    private void UpdateCountText()
    {
        _countText.text = _materialModel.Count.ToString(CountFormat);
    }

    private UniTask UpdateIconAsync(CancellationToken cancellationToken)
    {
        return SpriteLoader.LoadIntoAsync(_iconImage, _materialModel.IconKey, cancellationToken);
    }
}
