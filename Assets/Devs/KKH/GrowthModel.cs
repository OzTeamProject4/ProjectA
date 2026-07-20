using System.Collections.Generic;

public class GrowthModel : DataModel
{
    private readonly Dictionary<string, CharacterModel> _characters;

    public Inventory Inventory { get; }

    public GrowthModel(Inventory inventory, Dictionary<string, CharacterModel> characters)
    {
        Inventory = inventory;
        _characters = characters;
    }

    public CharacterModel GetCharacter(string characterId)
    {
        if (string.IsNullOrEmpty(characterId))
        {
            return null;
        }

        return _characters.TryGetValue(characterId, out CharacterModel model) ? model : null;
    }

    public IReadOnlyDictionary<string, CharacterModel> GetAllCharacters()
    {
        return _characters;
    }
}