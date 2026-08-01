using System.ComponentModel;
using UnityEngine;

public class StudentGachaModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs NameChanged = new PropertyChangedEventArgs(nameof(Name));
    private static readonly PropertyChangedEventArgs MainKeyChanged = new PropertyChangedEventArgs(nameof(MainKey));
    private static readonly PropertyChangedEventArgs PortraitKeyChanged = new PropertyChangedEventArgs(nameof(PortraitKey));

    private string _name;
    private string _mainKey;
    private string _portraitKey;
 
    public string Name
    {
        get { return _name; }
        private set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(NameChanged);
            }
        }
    }

    public string MainKey
    {
        get { return _mainKey; }
        private set
        {
            if (_mainKey != value)
            {
                _mainKey = value;
                OnPropertyChanged(MainKeyChanged);
            }
        }
    }

    public string PortraitKey
    {
        get { return _portraitKey; }
        private set
        {
            if (_portraitKey != value)
            {
                _portraitKey = value;
                OnPropertyChanged(PortraitKeyChanged);
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void UpdateGachaData(string gachaId)
    {
        if (string.IsNullOrWhiteSpace(gachaId))
        {
            Debug.LogError($"[{nameof(StudentGachaModel)}:{nameof(UpdateGachaData)}] 전달된 gachaId가 null입니다.");
            return;
        }

        if (!GameManager.Instance.DataManager.TryGetData(gachaId, out StudentGachaData studentGachaData))
        {
            Debug.LogError($"[{nameof(StudentGachaModel)}:{nameof(UpdateGachaData)}] '{gachaId}'에 해당하는 StudentGachaData를 찾을 수 없습니다.");
            return;
        }

        Name = studentGachaData.Name;
        MainKey = studentGachaData.MainKey;
        PortraitKey = studentGachaData.PortraitKey;
    }

    private void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(this, propertyChangedEventArgs);
    }
}