using System;
using System.Collections.Generic;
using UnityEngine;

public class PartySelectPopupViewModel
{
    private readonly List<PartySelectSlotViewModel> _items = new List<PartySelectSlotViewModel>();
    private readonly Dictionary<string, StudentModel> _modelById = new Dictionary<string, StudentModel>();

    public IReadOnlyList<PartySelectSlotViewModel> Items
    {
        get { return _items; }
    }

    public event Action<StudentModel> OnCharacterSelected;
    public event Action OnCloseRequested;

    public PartySelectPopupViewModel(IReadOnlyList<StudentModel> studentModel)
    {
        if (null == studentModel)
        {
            Debug.LogError("[PartySelectPopupViewModel] studentModel 가 null 입니다.");
            return;
        }

        foreach (StudentModel model in studentModel)
        {
            if (null == model)
            {
                continue;
            }

            _items.Add(new PartySelectSlotViewModel(model));
            _modelById[model.DataId] = model;
        }
    }

    public void SelectCommand(string characterId)
    {
        if (string.IsNullOrEmpty(characterId))
        {
            return;
        }

        if (!_modelById.TryGetValue(characterId, out StudentModel model))
        {
            Debug.LogWarning($"[PartySelectPopupViewModel] 캐릭터를 찾을 수 없습니다. id={characterId}");
            return;
        }

        OnCharacterSelected?.Invoke(model);
    }

    public void CloseCommand()
    {
        OnCloseRequested?.Invoke();
    }
}
