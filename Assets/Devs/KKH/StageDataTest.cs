using UnityEngine;

public class StageDataTest : MonoBehaviour
{
    private async void Start()
    {
        await GameManager.Instance.InitializeManagersAsync();

        IGameDataProvider dataProvider = new GameDataProvider();

        foreach (StageData stage in dataProvider.GetAllStages())
        {
            Debug.Log($"[StageDataTest] Stage={stage.DataId}, WaveCount={stage.WaveCount}, MapPrefabKey={stage.MapPrefabKey}");
        }

        foreach (StageWaveData wave in dataProvider.GetStageWaves("Stage_001"))
        {
            Debug.Log($"[StageDataTest] Stage_001 Wave={wave.WaveNumber}, MonsterIdList={wave.MonsterIdList}, MonsterCount={wave.MonsterCount}");

            if (wave.TryGetMonsters(out (string MonsterId, int Count)[] monsters))
            {
                foreach ((string monsterId, int count) in monsters)
                {
                    Debug.Log($"[StageDataTest]   → Monster={monsterId}, Count={count}");
                }
            }
        }
    }
}