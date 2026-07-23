using System;
using System.Collections.Generic;
using UnityEngine;

public class PartySelectPopupViewModel
{
    private readonly List<PartySelectItemViewModel> _items = new List<PartySelectItemViewModel>();
    private readonly Dictionary<string, CharacterModel> _modelById = new Dictionary<string, CharacterModel>();

    public IReadOnlyList<PartySelectItemViewModel> Items
    {
        get { return _items; }
    }

    public event Action<CharacterModel> OnCharacterSelected;
    public event Action OnCloseRequested;

    public PartySelectPopupViewModel(IReadOnlyList<CharacterModel> characterModels)
    {
        if (null == characterModels)
        {
            Debug.LogError("[PartySelectPopupViewModel] characterModels 가 null 입니다.");
            return;
        }

        foreach (CharacterModel model in characterModels)
        {
            if (null == model)
            {
                continue;
            }

            _items.Add(new PartySelectItemViewModel(model));
            _modelById[model.Id] = model;
        }
    }

    public void SelectCommand(string characterId)
    {
        if (string.IsNullOrEmpty(characterId))
        {
            return;
        }

        if (!_modelById.TryGetValue(characterId, out CharacterModel model))
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
