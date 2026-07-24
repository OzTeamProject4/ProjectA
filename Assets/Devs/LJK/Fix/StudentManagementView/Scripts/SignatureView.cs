using TMPro;
using UnityEngine;

public class SignatureView : EquipmentView
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;

    protected override void Awake()
    {
        base.Awake();
        UnityUtil.ValidateReference(_nameText, nameof(SignatureView), nameof(_nameText));
        UnityUtil.ValidateReference(_descriptionText, nameof(SignatureView), nameof(_descriptionText));
    }

    public override void UpdateView(EquipmentModel equipmentModel)
    {
        base.UpdateView(equipmentModel);
        _nameText.text = equipmentModel.Name;
        _descriptionText.text = equipmentModel.Description;
    }
}