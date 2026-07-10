using System;
using UnityEngine;

public class CharacterDetailViewModel
{
    private readonly CharacterModel _model;
    private readonly IGrowthDataProvider _dataProvider;

    private FinalStats _finalStats;

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
        get { return _dataProvider.GetRequiredExp(_model.CurrentLevel); }
    }

    public int RequiredDuplicatesForPromotion
    {
        get
        {
            CharacterGradeData grade = _dataProvider.GetGrade(_model.CurrentStar);
            return grade?.RequiredToNext ?? 0;
        }
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

    public CharacterDetailViewModel(CharacterModel model, IGrowthDataProvider dataProvider)
    {
        if (null == model)
        {
            Debug.LogError("CharacterModel 이 null 입니다.");
        }

        if (null == dataProvider)
        {
            Debug.LogError("IGrowthDataProvider 가 null 입니다.");
        }

        _model = model;
        _dataProvider = dataProvider;
    }

    public void Initialize()
    {
        _model.OnExpChanged += HandleModelChanged;
        _model.OnLevelChanged += HandleModelChanged;
        _model.OnStarChanged += HandleModelChanged;
        _model.OnDuplicatesChanged += HandleModelChanged;
        _model.OnItemCountChanged += HandleModelChanged;

        RefreshDisplay();
    }

    public void Dispose()
    {
        _model.OnExpChanged -= HandleModelChanged;
        _model.OnLevelChanged -= HandleModelChanged;
        _model.OnStarChanged -= HandleModelChanged;
        _model.OnDuplicatesChanged -= HandleModelChanged;
        _model.OnItemCountChanged -= HandleModelChanged;
    }

    public void UseExpItemCommand(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("UseExpItemCommand: itemId 가 비어 있습니다.");
            return;
        }

        ItemData item = _dataProvider.GetItem(itemId);
        if (null == item)
        {
            Debug.LogWarning($"UseExpItemCommand: ItemData 를 찾을 수 없습니다. ItemId={itemId}");
            return;
        }

        _model.UseExpItem(itemId, item.Value);
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

    public int GetItemCount(string itemId)
    {
        return _model.GetItemCount(itemId);
    }

    public string GetItemName(string itemId)
    {
        ItemData item = _dataProvider.GetItem(itemId);
        return item?.Name ?? itemId;
    }

    private void HandleModelChanged()
    {
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        CharacterStatData stat = _dataProvider.GetStat(_model.CharacterId);
        CharacterGradeData grade = _dataProvider.GetGrade(_model.CurrentStar);

        _finalStats = StatCalculator.Calculate(stat, grade, _model.CurrentLevel);

        OnDisplayChanged?.Invoke();
    }
}