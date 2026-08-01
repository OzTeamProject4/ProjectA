using System;
using System.ComponentModel;
using UnityEngine;

public class PlayerProfileModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs IntroductionChanged = new PropertyChangedEventArgs(nameof(Introduction));
    private static readonly PropertyChangedEventArgs RepresentativeStudentIdChanged = new PropertyChangedEventArgs(nameof(RepresentativeStudentId));

    private string _nickname;
    private int _level;
    private string _title;
    private DateTime _accountCreatedAt;
    private DateTime _lastConnectAt;
    private int _totalLoginDays;
    private string _introduction;
    private int _mainStoryChapter;
    private int _mainStoryStage;
    private string _representativeStudentId;

    public string Nickname
    {
        get
        {
            return _nickname;
        }
    }

    public int Level
    {
        get
        {
            return _level;
        }
    }

    public string Title
    {
        get
        {
            return _title;
        }
    }

    public DateTime AccountCreatedAt
    {
        get
        {
            return _accountCreatedAt;
        }
    }

    public DateTime LastConnectAt
    {
        get
        {
            return _lastConnectAt;
        }
    }

    public int TotalLoginDays
    {
        get
        {
            return _totalLoginDays;
        }
    }

    public string Introduction
    {
        get
        {
            return _introduction;
        }
        private set
        {
            if (_introduction == value)
            {
                return;
            }

            _introduction = value;
            OnPropertyChanged(IntroductionChanged);
        }
    }

    public int MainStoryChapter
    {
        get
        {
            return _mainStoryChapter;
        }
    }

    public int MainStoryStage
    {
        get
        {
            return _mainStoryStage;
        }
    }

    public string RepresentativeStudentId
    {
        get
        {
            return _representativeStudentId;
        }
        private set
        {
            if (_representativeStudentId == value)
            {
                return;
            }

            _representativeStudentId = value;
            OnPropertyChanged(RepresentativeStudentIdChanged);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public PlayerProfileModel(string nickname, int level, string title, DateTime accountCreatedAt, DateTime lastConnectAt, int totalLoginDays, string introduction, int mainStoryChapter, int mainStoryStage, string representativeStudentId)
    {
        _nickname = nickname;
        _level = level;
        _title = title;
        _accountCreatedAt = accountCreatedAt;
        _lastConnectAt = lastConnectAt;
        _totalLoginDays = totalLoginDays;
        _introduction = introduction;
        _mainStoryChapter = mainStoryChapter;
        _mainStoryStage = mainStoryStage;
        _representativeStudentId = representativeStudentId;
    }

    public void SetIntroduction(string introduction)
    {
        Introduction = introduction;
    }

    public void SetRepresentativeStudentId(string representativeStudentId)
    {
        if (string.IsNullOrEmpty(representativeStudentId))
        {
            Debug.LogWarning($"[{nameof(PlayerProfileModel)}:{nameof(SetRepresentativeStudentId)}] 대표 학생 ID가 비어 있어 변경하지 않습니다.");
            return;
        }

        RepresentativeStudentId = representativeStudentId;
    }

    protected void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(this, propertyChangedEventArgs);
    }
}
