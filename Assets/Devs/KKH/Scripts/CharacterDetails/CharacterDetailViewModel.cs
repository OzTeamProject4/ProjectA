using System;
using UnityEngine;

public class CharacterDetailViewModel
{
    private readonly CharacterModel _model;

    private StatData _finalStats;

    public string CharacterId
    {
        get { return _model.CharacterId; }
    }

    public int CurrentStar
    {
        get { return _model.CurrentStar; }
    }

    public int CurrentLevel
    {
        get { return _model.CurrentLevel; }
    }

    public int CurrentExp
    {
        get { return _model.CurrentExp; }
    }

    public bool IsMaxLevel
    {
        get { return _model.IsMaxLevel; }
    }

    public bool IsMaxStar
    {
        get { return _model.IsMaxStar; }
    }

    public int OwnedDuplicates
    {
        get { return _model.OwnedDuplicates; }
    }

    public bool CanPromote
    {
        get { return _model.CanPromote(); }
    }

    public int RequiredExpForNextLevel
    {
        get { return _model.GetRequiredExpForNextLevel(); }
    }

    public int RequiredDuplicatesForPromotion
    {
        get { return _model.GetRequiredDuplicatesForPromotion(); }
    }

    public int DisplayHp
    {
        get { return Mathf.RoundToInt(_finalStats.Hp); }
    }

    public int DisplayAtk
    {
        get { return Mathf.RoundToInt(_finalStats.Atk); }
    }

    public int DisplayDef
    {
        get { return Mathf.RoundToInt(_finalStats.Def); }
    }

    public float DisplayAtkSpeed
    {
        get { return _finalStats.AtkSpeed; }
    }

    public float DisplayMoveSpeed
    {
        get { return _finalStats.MoveSpeed; }
    }

    public event Action OnDisplayChanged;

    public CharacterDetailViewModel(CharacterModel model)
    {
        if (null == model)
        {
            Debug.LogError("CharacterModel 이 null 입니다.");
        }

        _model = model;
    }

    public void Initialize()
    {
        _model.OnExpChanged += HandleModelChanged;
        _model.OnLevelChanged += HandleModelChanged;
        _model.OnStarChanged += HandleModelChanged;
        _model.OnDuplicatesChanged += HandleModelChanged;
        _model.OnEquipmentChanged += HandleModelChanged;

        RefreshDisplay();
    }

    public void Dispose()
    {
        _model.OnExpChanged -= HandleModelChanged;
        _model.OnLevelChanged -= HandleModelChanged;
        _model.OnStarChanged -= HandleModelChanged;
        _model.OnDuplicatesChanged -= HandleModelChanged;
        _model.OnEquipmentChanged -= HandleModelChanged;
    }

    public void UseExpItemCommand(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("UseExpItemCommand: itemId 가 비어 있습니다.");
            return;
        }

        _model.UseExpItem(itemId);
    }

    public void PromoteCommand()
    {
        if (!_model.CanPromote())
        {
            Debug.LogWarning("PromoteCommand: 승급 조건을 만족하지 않습니다.");
            return;
        }

        _model.Promote();
    }

    private void HandleModelChanged()
    {
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        _finalStats = _model.GetFinalStats();
        OnDisplayChanged?.Invoke();
    }
}