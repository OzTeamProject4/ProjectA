using System;
using UnityEngine;

public class CharacterListItemViewModel
{
    private readonly CharacterModel _model;

    public string DataId
    {
        get { return _model.CharacterId; }
    }

    public int CurrentStar
    {
        get { return _model.CurrentStar; }
    }

    public event Action OnStarChanged;

    public CharacterListItemViewModel(CharacterModel model)
    {
        if (null == model)
        {
            Debug.LogError("CharacterModel 이 null 입니다.");
        }

        _model = model;
    }

    public void Initialize()
    {
        _model.OnStarChanged += HandleStarChanged;
    }

    public void Dispose()
    {
        _model.OnStarChanged -= HandleStarChanged;
    }

    private void HandleStarChanged()
    {
        OnStarChanged?.Invoke();
    }
}