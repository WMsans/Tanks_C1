using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoSingleton<EnemySpawner>
{
    [SerializeField] private ArenaPointSampler sampler;

    public void SpawnTanksInArena(GameObject prefab, int cnt)
    {
        for (var i = 1; i <= cnt; i++)
        {
            SpawnTankInArena(prefab);
        }
    }

    private void SpawnTankInArena(GameObject prefab)
    {
        SpawnTank(prefab, sampler.GetNavMeshPointInBound(), Quaternion.identity, false);
    }

    private void SpawnTank(GameObject prefab, Vector3 spawnPos, Quaternion spawnRot, bool isPlayer)
    {
        if (!isPlayer)
        {
            Instantiate(prefab, spawnPos, spawnRot);
        }
    }
}
