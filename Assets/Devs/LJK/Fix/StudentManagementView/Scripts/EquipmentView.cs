using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentView : BaseButton
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private EquipType _equipType;

    public EquipType EquipType
    {
        get 
        { 
            return _equipType; 
        }
    }

    public event Action<EquipType> OnButtonClicked;

    protected override void Awake()
    {
        base.Awake();

        UnityUtil.ValidateReference(_iconImage, nameof(EquipmentView), nameof(_iconImage));
    }

    public async UniTask UpdateIconAsync(string iconPath)
    {
        if (string.IsNullOrWhiteSpace(iconPath))
        {
            Debug.LogError("장비 아이콘 경로가 비어 있습니다.");
            return;
        }

        Sprite iconSprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(iconPath, destroyCancellationToken);

        if (iconSprite == null)
        {
            Debug.LogError($"'{iconPath}' 장비 아이콘을 불러오지 못했습니다.");
            return;
        }

        _iconImage.sprite = iconSprite;
    }

    public virtual void UpdateView(EquipmentModel equipmentModel)
    {
        if (equipmentModel == null)
        {
            Debug.LogError("장비 모델이 null입니다.");
            return;
        }

        UpdateIconAsync(equipmentModel.IconKey).Forget();
    }

    protected override void OnButtonClick()
    {
        if (OnButtonClicked == null)
        {
            return;
        }

        OnButtonClicked.Invoke(_equipType);
    }
}