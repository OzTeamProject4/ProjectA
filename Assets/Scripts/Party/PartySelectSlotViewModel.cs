using UnityEngine;

public class PartySelectSlotViewModel
{
    private readonly StudentModel _model;
    private readonly int _assignedSlotNumber;

    public int AssignedSlotNumber
    {
        get { return _assignedSlotNumber; }
    }

    public bool IsAssigned
    {
        get { return _assignedSlotNumber > 0; }
    }

    public string DataId
    {
        get
        {
            if (null == _model)
            {
                return string.Empty;
            }

            return _model.DataId;
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

            return _model.PortraitKey;
        }
    }

    public PartySelectSlotViewModel(StudentModel model, int assignedSlotNumber)
    {
        if (null == model)
        {
            Debug.LogError("[PartySelectItemViewModel] model 이 null 입니다.");
        }

        _model = model;
        _assignedSlotNumber = assignedSlotNumber;
    }
}
