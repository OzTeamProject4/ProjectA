using UnityEngine;

public interface ICharacterStatProvider
{
    StatData GetFinalStats(string characterId);
}

public class MockCharacterStatProvider : ICharacterStatProvider
{
    private const int TempStar = 3;
    private const int TempLevel = 1;

    private readonly IGameDataProvider _dataProvider;

    public MockCharacterStatProvider(IGameDataProvider dataProvider)
    {
        if (null == dataProvider)
        {
            Debug.LogError("[MockCharacterStatProvider] dataProvider 가 null 입니다.");
        }

        _dataProvider = dataProvider;
    }

    public StatData GetFinalStats(string characterId)
    {
        if (string.IsNullOrEmpty(characterId))
        {
            Debug.LogWarning("[MockCharacterStatProvider:GetFinalStats] characterId 가 비어 있습니다.");
            return default;
        }

        CharacterData stat = _dataProvider.GetStat(characterId);
        CharacterGradeData grade = _dataProvider.GetGrade(TempStar);

        return StatCalculator.Calculate(stat, grade, TempLevel, default);
    }
}

