using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ProfileViewModel
{
    private static readonly string[] CurrencyItemIds =
    {
        CurrencyItemId.Gold,
        CurrencyItemId.Crystal,
        CurrencyItemId.MaterialT1
    };

    private readonly List<MaterialModel> _currencyList = new List<MaterialModel>();

    private PlayerProfileModel _playerProfileModel;
    private InventoryModel _inventoryModel;
    private StudentListModel _studentListModel;
    private StageClearModel _stageClearModel;

    public IReadOnlyList<MaterialModel> CurrencyList
    {
        get
        {
            return _currencyList;
        }
    }

    public string Nickname
    {
        get
        {
            return _playerProfileModel.Nickname;
        }
    }

    public int Level
    {
        get
        {
            return _playerProfileModel.Level;
        }
    }

    public DateTime AccountCreatedAt
    {
        get
        {
            return _playerProfileModel.AccountCreatedAt;
        }
    }

    public DateTime LastConnectAt
    {
        get
        {
            return _playerProfileModel.LastConnectAt;
        }
    }

    public int TotalLoginDays
    {
        get
        {
            return _playerProfileModel.TotalLoginDays;
        }
    }

    public string Introduction
    {
        get
        {
            return _playerProfileModel.Introduction;
        }
    }

    public int MainStoryChapter
    {
        get
        {
            return _playerProfileModel.MainStoryChapter;
        }
    }

    public int MainStoryStage
    {
        get
        {
            return _playerProfileModel.MainStoryStage;
        }
    }

    public int OwnedStudentCount
    {
        get
        {
            return _studentListModel.StudentList.Count;
        }
    }

    public int ClearedStageCount
    {
        get
        {
            return _stageClearModel.ClearedStageCount;
        }
    }

    public string RepresentativeStudentFullBodyKey
    {
        get
        {
            StudentModel studentModel = GetRepresentativeStudentModel();

            if (studentModel == null)
            {
                return string.Empty;
            }

            return studentModel.FullBodyKey;
        }
    }

    public event Action<string> PropertyChanged;

    public ProfileViewModel()
    {
        _playerProfileModel = NetworkManager.Instance.PlayerProfileModel;
        _playerProfileModel.PropertyChanged += OnProfileModelChanged;

        _inventoryModel = NetworkManager.Instance.InventoryModel;
        _inventoryModel.PropertyChanged += OnInventoryModelChanged;

        _studentListModel = NetworkManager.Instance.StudentListModel;

        _stageClearModel = NetworkManager.Instance.StageClearModel;
        _stageClearModel.PropertyChanged += OnStageClearModelChanged;

        BuildCurrencyList();
    }

    public void Refresh()
    {
        OnPropertyChanged(nameof(Nickname));
        OnPropertyChanged(nameof(Level));
        OnPropertyChanged(nameof(AccountCreatedAt));
        OnPropertyChanged(nameof(LastConnectAt));
        OnPropertyChanged(nameof(TotalLoginDays));
        OnPropertyChanged(nameof(Introduction));
        OnPropertyChanged(nameof(MainStoryChapter));
        OnPropertyChanged(nameof(MainStoryStage));
        OnPropertyChanged(nameof(CurrencyList));
        OnPropertyChanged(nameof(OwnedStudentCount));
        OnPropertyChanged(nameof(ClearedStageCount));
        OnPropertyChanged(nameof(RepresentativeStudentFullBodyKey));
    }

    public void SetRepresentativeStudentId(string representativeStudentId)
    {
        _playerProfileModel.SetRepresentativeStudentId(representativeStudentId);
    }

    public void SetIntroduction(string introduction)
    {
        _playerProfileModel.SetIntroduction(introduction);
    }

    public void Dispose()
    {
        if (_playerProfileModel != null)
        {
            _playerProfileModel.PropertyChanged -= OnProfileModelChanged;
            _playerProfileModel = null;
        }

        if (_inventoryModel != null)
        {
            _inventoryModel.PropertyChanged -= OnInventoryModelChanged;
            _inventoryModel = null;
        }

        if (_stageClearModel != null)
        {
            _stageClearModel.PropertyChanged -= OnStageClearModelChanged;
            _stageClearModel = null;
        }

        _studentListModel = null;
        _currencyList.Clear();
    }

    private StudentModel GetRepresentativeStudentModel()
    {
        string representativeStudentId = _playerProfileModel.RepresentativeStudentId;

        if (string.IsNullOrEmpty(representativeStudentId))
        {
            return null;
        }

        StudentModel studentModel = _studentListModel.GetCharacter(representativeStudentId);

        if (studentModel == null)
        {
            Debug.LogWarning($"[{nameof(ProfileViewModel)}:{nameof(GetRepresentativeStudentModel)}] 보유하지 않은 대표 학생입니다. DataId={representativeStudentId}");
        }

        return studentModel;
    }

    private void BuildCurrencyList()
    {
        _currencyList.Clear();

        foreach (string currencyItemId in CurrencyItemIds)
        {
            if (!_inventoryModel.TryGetMaterial(currencyItemId, out MaterialModel materialModel))
            {
                Debug.LogWarning($"[{nameof(ProfileViewModel)}:{nameof(BuildCurrencyList)}] '{currencyItemId}'를 인벤토리에서 찾을 수 없습니다.");
                continue;
            }

            _currencyList.Add(materialModel);
        }
    }

    private void OnProfileModelChanged(object sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName);

        if (e.PropertyName == nameof(PlayerProfileModel.RepresentativeStudentId))
        {
            OnPropertyChanged(nameof(RepresentativeStudentFullBodyKey));
        }
    }

    private void OnStageClearModelChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(StageClearModel.ClearedStageCount))
        {
            return;
        }

        OnPropertyChanged(nameof(ClearedStageCount));
    }

    private void OnInventoryModelChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(InventoryModel.Inventory))
        {
            return;
        }

        BuildCurrencyList();
        OnPropertyChanged(nameof(CurrencyList));
    }

    private void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(propertyName);
    }
}
