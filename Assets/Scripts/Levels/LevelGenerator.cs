using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private string name;
    public string Name => name;

    public void GenerateLevel(EnemySpawner enemySpawner, ItemSpawner itemSpawner)
    {
        foreach (var x in waveEnemies)
        {
            if (enemySpawner != null) enemySpawner.SpawnTanksInArena(x.enemyPrefab, x.enemyNum);
        }

        foreach (var x in waveItems)
        {
            if (itemSpawner != null) itemSpawner.SpawnItemInArena(x.itemPrefab, x.itemNum);
        }
    }

    public int GetEnemyNum()
    {
        var cnt = 0;
        foreach (var x in waveEnemies) cnt += x.enemyNum;
        return cnt;
    }
}
