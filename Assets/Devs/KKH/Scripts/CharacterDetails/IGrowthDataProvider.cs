using System.Collections.Generic;

public interface IGrowthDataProvider
{
    public CharacterStatData GetStat(string characterId);
    public CharacterGradeData GetGrade(int star);
    public ItemData GetItem(string dataId);

    public int GetRequiredExp(int level);
    public IReadOnlyList<string> GetAllCharacterIds();
    public IReadOnlyList<string> GetAllExpItemIds();
}