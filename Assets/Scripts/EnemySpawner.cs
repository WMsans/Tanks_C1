using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoSingleton<EnemySpawner>
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private ArenaPointSampler sampler;

    private void OnEnable()
    {
        SpawnTankInArena();
    }

    public void SpawnTanksInArena(int cnt)
    {
        for (int i = 1; i <= cnt; i++)
        {
            SpawnTankInArena();
        }
    }
    public void SpawnTankInArena()
    {
        SpawnTank(sampler.GetNavMeshPointInBound(), Quaternion.identity, false);
    }

    private void SpawnTank(Vector3 spawnPos, Quaternion spawnRot, bool isPlayer)
    {
        if (!isPlayer)
        {
            Instantiate(enemyPrefab, spawnPos, spawnRot);
        }
    }
}
