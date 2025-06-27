using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoSingleton<ItemSpawner>
{
    [SerializeField] private ArenaPointSampler sampler;

    public void SpawnItemInArena(GameObject prefab, int cnt)
    {
        for(var i = 0; i < cnt;i++)
        {
            SpawnItem(prefab, sampler.GetNavMeshPointInBound(), Quaternion.identity, false);
        }
    }

    private void SpawnItem(GameObject prefab, Vector3 spawnPos, Quaternion spawnRot, bool isPlayer)
    {
        if (!isPlayer)
        {
            Instantiate(prefab, spawnPos, spawnRot);
        }
    }
}
