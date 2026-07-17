using System.Collections.Generic;
using UnityEngine;

public class StageSelectController
{
    private readonly StageMonsterParty _partyPrefab;
    private readonly StageSelectMap _map;
    private readonly StageProgressModel _progressModel;

    private readonly List<StageMonsterParty> _spawnedParties = new List<StageMonsterParty>();

    public StageSelectController(StageMonsterParty partyPrefab, StageSelectMap map, StageProgressModel progressModel)
    {
        if (null == partyPrefab || null == map || null == progressModel)
        {
            Debug.LogError("[StageSelectController] partyPrefab, map 또는 progressModel 이 null 입니다.");
        }

        _partyPrefab = partyPrefab;
        _map = map;
        _progressModel = progressModel;
    }

    public void SpawnParties()
    {
        if (null == _map || null == _partyPrefab)
        {
            return;
        }

        foreach (MonsterPartySpawner spawner in _map.Spawners)
        {
            if (null == spawner)
            {
                continue;
            }

            SpawnPartyAt(spawner);
        }
    }

    public void Dispose()
    {
        foreach (StageMonsterParty party in _spawnedParties)
        {
            if (null == party)
            {
                continue;
            }

            party.OnPlayerReached -= HandlePlayerReached;
        }

        _spawnedParties.Clear();
    }

    private void SpawnPartyAt(MonsterPartySpawner spawner)
    {
        StageMonsterParty party = Object.Instantiate(_partyPrefab, spawner.SpawnPosition, Quaternion.identity);

        party.SetStageId(spawner.StageId);

        party.OnPlayerReached -= HandlePlayerReached;
        party.OnPlayerReached += HandlePlayerReached;

        _spawnedParties.Add(party);
    }

    private void HandlePlayerReached(string stageId)
    {
        _progressModel.SelectStage(stageId);

        Debug.Log($"[StageSelectController] 플레이어 도착 → 스테이지 선택: {stageId}");
    }
}
