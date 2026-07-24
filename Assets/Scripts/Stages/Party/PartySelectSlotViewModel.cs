using UnityEngine;

public class PartySelectSlotViewModel
{
    private readonly CharacterModel _model;

    public string DataId
    {
        get
        {
            if (null == _model)
            {
                return string.Empty;
            }

            return _model.Id;
        }
    }

    public string Name
    {
        get
        {
            if (null == _model)
            {
                return string.Empty;
            }

            return _model.Name;
        }
    }

    public int Star
    {
        get
        {
            if (null == _model)
            {
                return 0;
            }

            return _model.Star;
        }
    }

    public string IconPath
    {
        get
        {
            if (null == _model)
            {
                return null;
            }

            return _model.IconPath;
        }
    }

    public PartySelectSlotViewModel(CharacterModel model)
    {
        if (null == model)
        {
            Debug.LogError("[PartySelectItemViewModel] model 이 null 입니다.");
        }

        _model = model;
    }
}
