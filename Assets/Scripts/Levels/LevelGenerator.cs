using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Config", fileName = "New Level Config")]
public class LevelGenerator : ScriptableObject, ILevelGenerator
{
    [System.Serializable]
    private struct EnemySpawnWaveConfig
    {
        public GameObject enemyPrefab;
        public int enemyNum;
    }
    
    [SerializeField] private List<EnemySpawnWaveConfig> wave;
    public void GenerateLevel()
    {
        var spawner = EnemySpawner.Instance;
        foreach (var x in wave)
        {
            spawner.SpawnTanksInArena(x.enemyPrefab, x.enemyNum);
        }
    }

    public int GetEnemyNum()
    {
        var cnt = 0;
        foreach (var x in wave) cnt += x.enemyNum;
        return cnt;
    }
}
