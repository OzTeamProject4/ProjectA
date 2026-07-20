using System.Collections.Generic;
using UnityEngine;

public class CharacterListViewModel
{
    private readonly List<CharacterListItemViewModel> _items = new();

    public IReadOnlyList<CharacterListItemViewModel> Items
    {
        get { return _items; }
    }

    public CharacterListViewModel(IReadOnlyList<CharacterModel> characterModels)
    {
        if (null == characterModels)
        {
            Debug.LogError("characterModels 가 null 입니다.");
            return;
        }

        foreach (CharacterModel model in characterModels)
        {
            _items.Add(new CharacterListItemViewModel(model));
        }
    }
}