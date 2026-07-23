//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using UnityEngine;

//public class ItemPreviewPopupViewModel
//{
//    private ItemModel _itemModel;

//    public string Name
//    {
//        get { return _itemModel.ItemName; }
//    }

//    public string IconPath
//    {
//        get { return _itemModel.IconPath; }
//    }

//    public IReadOnlyList<StatInfo> StatInfo
//    {
//        get { return _itemModel.StatInfo; }
//    }

//    public event Action<string> ModelPropertyChanged;

//    public void Init(ItemModel itemModel)
//    {
//        if (_itemModel != null)
//        {
//            _itemModel.PropertyChanged -= OnModelPropertyChanged;
//        }

//        _itemModel = itemModel;
//        _itemModel.PropertyChanged += OnModelPropertyChanged;
//    }

//    public void Dispose()
//    {
//        if (_itemModel != null)
//        {
//            _itemModel.PropertyChanged -= OnModelPropertyChanged;
//            _itemModel = null;
//        }
//    }

//    private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
//    {
//        if (ModelPropertyChanged == null)
//        {
//            return;
//        }

//        ModelPropertyChanged.Invoke(e.PropertyName);
//    }
//}