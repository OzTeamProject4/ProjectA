using UnityEngine;

public interface ICharacterInfoProvider
{
    StatData GetFinalStats(string characterId);
    string GetIconPath(string characterId);
}

public class MockCharacterInfoProvider : ICharacterInfoProvider
{
    private const int TempStar = 3;
    private const int TempLevel = 1;

    private readonly IGameDataProvider _dataProvider;

    public MockCharacterInfoProvider(IGameDataProvider dataProvider)
    {
        if (null == dataProvider)
        {
            Debug.LogError("[MockCharacterInfoProvider] dataProvider 가 null 입니다.");
        }

        _dataProvider = dataProvider;
    }

    public StatData GetFinalStats(string characterId)
    {
        if (string.IsNullOrEmpty(characterId))
        {
            Debug.LogWarning("[MockCharacterInfoProvider:GetFinalStats] characterId 가 비어 있습니다.");
            return default;
        }

        CharacterData stat = _dataProvider.GetStat(characterId);
        CharacterGradeData grade = _dataProvider.GetGrade(TempStar);

        return StatCalculator.Calculate(stat, grade, TempLevel, default);
    }

    // 임의 캐릭터의 초상화 경로 조회 (도감/파티편성 등에서 재사용)
    public string GetIconPath(string characterId)
    {
        if (string.IsNullOrEmpty(characterId))
        {
            Debug.LogWarning("[MockCharacterInfoProvider:GetIconPath] characterId 가 비어 있습니다.");
            return null;
        }

        CharacterData characterData = _dataProvider.GetStat(characterId);

        if (null == characterData)
        {
            return null;
        }

        return characterData.CharacterIconPath;
    }
}