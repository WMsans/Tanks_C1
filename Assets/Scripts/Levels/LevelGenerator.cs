using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Level Config", fileName = "New Level Config")]
public class LevelGenerator : ScriptableObject, ILevelGenerator
{
    [System.Serializable]
    private struct EnemySpawnWaveConfig
    {
        public GameObject enemyPrefab;
        public int enemyNum;
    }
    [System.Serializable]
    private struct ItemSpawnWaveConfig
    {
        public GameObject itemPrefab;
        public int itemNum;
    }
    
    [FormerlySerializedAs("wave")] [SerializeField] private List<EnemySpawnWaveConfig> waveEnemies;
    [SerializeField] private List<ItemSpawnWaveConfig> waveItems;
    public void GenerateLevel()
    {
        var spawner = EnemySpawner.Instance;
        foreach (var x in waveEnemies)
        {
            spawner.SpawnTanksInArena(x.enemyPrefab, x.enemyNum);
        }

        var itemSpawner = ItemSpawner.Instance;
        foreach (var x in waveItems)
        {
            itemSpawner.SpawnItemInArena(x.itemPrefab, x.itemNum);
        }
    }

    public int GetEnemyNum()
    {
        var cnt = 0;
        foreach (var x in waveEnemies) cnt += x.enemyNum;
        return cnt;
    }
}
