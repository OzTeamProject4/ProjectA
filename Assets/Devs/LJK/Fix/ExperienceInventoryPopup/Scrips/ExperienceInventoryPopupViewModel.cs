using System;
using System.Collections.Generic;
using System.ComponentModel;

public class ExperienceInventoryPopupViewModel
{
    private StudentModel _studentModel;
    private InventoryModel _inventoryModel;

    public IReadOnlyDictionary<string, MaterialModel> ExperienceItems { get; private set; }   

    public event Action<string> PropertyChanged;

    public ExperienceInventoryPopupViewModel()
    {
        _inventoryModel = NetworkManagerTemp.Instance.InventoryModel;
    }

    public void SetModel(StudentModel characterModel)
    {
        if (_studentModel != null)
        {
            _studentModel.PropertyChanged -= OnPropertyChanged;
        }

        _studentModel = characterModel;
        _studentModel.PropertyChanged += OnPropertyChanged;

        ExperienceItems = _inventoryModel.GetItemsByMaterialType(MaterialType.Exp);
    }

    public void UseExpItem(MaterialModel materialModel)
    {
        materialModel.UseExpItem(_studentModel);
    }

    public void Dispose()
    {
        _inventoryModel = null;

        if (_studentModel == null)
        {
            return;
        }

        _studentModel.PropertyChanged -= OnPropertyChanged;
        _studentModel = null;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(e.PropertyName);
    }
}