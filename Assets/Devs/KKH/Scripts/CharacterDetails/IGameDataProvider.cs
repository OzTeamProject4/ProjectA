using System.Collections.Generic;

public interface IGameDataProvider
{
    CharacterStatData GetStat(string characterId);
    CharacterGradeData GetGrade(int star);
    ItemData GetItem(string dataId);

    int GetRequiredExp(int level);
    IReadOnlyList<string> GetAllCharacterIds();
    IReadOnlyList<string> GetAllExpItemIds();

    EquipmentData GetEquipment(string dataId);
    IReadOnlyList<EquipmentData> GetAllEquipment();
}