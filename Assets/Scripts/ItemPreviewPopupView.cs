//using Cysharp.Threading.Tasks;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class ItemPreviewPopupView : BaseUI
//{
//    [SerializeField] private TMP_Text _nameText;
//    [SerializeField] private TMP_Text _descriptionText;
//    [SerializeField] private Image _iconImage;

//    //TODO List로
//    [SerializeField] private StatItemView[] _statRows;

//    [SerializeField] private RectTransform _cardRect;

//    private ItemPreviewPopupViewModel _viewModel;

//    private CancellationTokenSource _disableCts;

//    public void Bind(string itemid, Vector3 position)
//    {
//        if(!GameManager.Instance.DataManager.TryGetData(itemid, out EquipmentData equipmentData))
//        {
//            Debug.LogError("");
//        }

//        ItemModel itemModel = new ItemModel(itemid, equipmentData.SpritePath, 1, equipmentData.Type, 1);

//        _viewModel.Init(itemModel);

//        LoadIconAsync().Forget();
//        _nameText.text = _viewModel.Name;
//        MoveCardTo(position);
//        RefreshStatRows();
//    }

//    private void Awake()
//    {
//        UnityUtil.ValidateReference(_nameText, nameof(EquipmentDetailPopupView), nameof(_nameText));
//        UnityUtil.ValidateReference(_descriptionText, nameof(EquipmentDetailPopupView), nameof(_descriptionText));
//        UnityUtil.ValidateReference(_iconImage, nameof(EquipmentDetailPopupView), nameof(_iconImage));

//        _viewModel = new ItemPreviewPopupViewModel();
//    }

//    private void OnEnable()
//    {
//        _viewModel.ModelPropertyChanged += OnModelPropertyChanged;
//        _disableCts = new CancellationTokenSource();
//    }

//    private void OnDisable()
//    {
//        _viewModel.ModelPropertyChanged -= OnModelPropertyChanged;

//        _disableCts?.Cancel();
//        _disableCts?.Dispose();
//        _disableCts = null;
//    }

 

//    //TODO 필요없을시 제거
//    private void OnModelPropertyChanged(string propertyName)
//    {
//        switch (propertyName)
//        {
//            case nameof(_viewModel.IconPath):
//                //UpdateList();
//                break;
//        }
//    }

//    private async UniTask LoadIconAsync()
//    {
//        string iconPath = _viewModel.IconPath;

//        if (string.IsNullOrWhiteSpace(iconPath))
//        {
//            Debug.LogError($"아이콘 경로가 비어 있습니다.");
//            return;
//        }

//        Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(iconPath, _disableCts.Token);

//        if (sprite == null)
//        {
//            Debug.LogError($"아이콘을 로드하지 못했습니다. Path: {iconPath}");
//            return;
//        }

//        _iconImage.sprite = sprite;
//    }

//    public void MoveCardTo(Vector3 worldPosition)
//    {
//        _cardRect.pivot = new Vector2(1, 1);
//        _cardRect.position = worldPosition;
//    }

//    //List로 바꾸기
//    private void RefreshStatRows()
//    {
//        IReadOnlyList<StatInfo> info = _viewModel.StatInfo;

//        for (int i = 0; i < _statRows.Length; i++)
//        {
//            if (null == _statRows[i])
//            {
//                continue;
//            }

//            if (i >= info.Count)
//            {
//                _statRows[i].Hide();
//                continue;
//            }

//            _statRows[i].SetValue(info[i].Type, info[i].Value);
//        }
//    }
//}