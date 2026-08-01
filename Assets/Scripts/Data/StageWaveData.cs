public class StageWaveData : BaseData
{
    public string StageId { get; init; }
    public int WaveNumber { get; init; }
    public string MonsterIdList { get; init; }
    public string MonsterCount { get; init; }

    private string[] _monsterIds;
    private int[] _monsterCounts;

    public string[] MonsterIds
    {
        get
        {
            if (null == _monsterIds)
            {
                _monsterIds = Util.ParseIds(MonsterIdList);
            }

            return _monsterIds;
        }
    }

    public int[] MonsterCounts
    {
        get
        {
            if (null == _monsterCounts)
            {
                _monsterCounts = Util.ParseCounts(MonsterCount);
            }

            return _monsterCounts;
        }
    }

    public bool TryGetMonsters(out (string MonsterId, int Count)[] monsters)
    {
        if (!Util.TryPairMaterials(MonsterIds, MonsterCounts, out (string ItemId, int Count)[] pairs))
        {
            monsters = System.Array.Empty<(string, int)>();
            return false;
        }

        monsters = new (string, int)[pairs.Length];
        for (int i = 0; i < pairs.Length; i++)
        {
            monsters[i] = (pairs[i].ItemId, pairs[i].Count);
        }

        return true;
    }
}