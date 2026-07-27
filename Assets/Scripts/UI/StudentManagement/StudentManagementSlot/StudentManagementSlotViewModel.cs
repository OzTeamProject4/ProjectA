using System;
using System.ComponentModel;
using UnityEngine;

public class StudentManagementSlotViewModel
{
    private StudentModel _studentModel;

    public StudentModel StudentModel
    {
        get
        {
            return _studentModel;
        }
    }

    public string Name
    {
        get
        {
            return _studentModel.Name;
        }
    }

    public int Star
    {
        get
        {
            return _studentModel.Star;
        }
    }

    public string PortraitKey
    {
        get
        {
            return _studentModel.PortraitKey;
        }
    }

    public event Action<string> PropertyChanged;

    public void SetModel(StudentModel studentModel)
    {
        if (studentModel == null)
        {
            Debug.LogError($"{nameof(StudentManagementSlotViewModel)} SetModel 실패: studentModel이 null입니다.");
            return;
        }

        if (ReferenceEquals(_studentModel, studentModel))
        {
            return;
        }

        if (_studentModel != null)
        {
            _studentModel.PropertyChanged -= OnPropertyChanged;
        }

        _studentModel = studentModel;
        _studentModel.PropertyChanged += OnPropertyChanged;
    }

    public void Dispose()
    {
        if (_studentModel == null)
        {
            return;
        }

        _studentModel.PropertyChanged -= OnPropertyChanged;
        _studentModel = null;
    }

    public void Refresh()
    {
        if (_studentModel == null)
        {
            return;
        }

        _studentModel.NotifyAllProperties();
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