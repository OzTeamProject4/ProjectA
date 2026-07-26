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

    public UniTask UpdateIconAsync(string iconKey)
    {
        return SpriteLoader.LoadIntoAsync(_iconImage, iconKey, destroyCancellationToken);
    }

    public virtual void ClearView()
    {
        _iconImage.enabled = false;
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