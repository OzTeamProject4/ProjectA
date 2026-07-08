using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

public interface IGrowthDataProvider
{
    UniTask InitializeAsync(CancellationToken token);

    CharacterStatData GetStat(string characterId);
    CharacterGradeData GetGrade(int star);
    ItemData GetItem(string itemId);

    public int GetRequiredExp(int level);
    IReadOnlyList<string> GetAllCharacterIds();
}