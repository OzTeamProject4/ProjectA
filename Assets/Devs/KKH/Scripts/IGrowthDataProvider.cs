using System.Collections.Generic;

public interface IGrowthDataProvider
{
    CharacterStatData GetStat(string characterId);
    CharacterGradeData GetGrade(int star);
    ItemData GetItem(string dataId);

    public int GetRequiredExp(int level);
    IReadOnlyList<string> GetAllCharacterIds();
    IReadOnlyList<string> GetAllExpItemIds();
}