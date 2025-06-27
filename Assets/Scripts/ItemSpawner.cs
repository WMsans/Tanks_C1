using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoSingleton<ItemSpawner>
{
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private ArenaPointSampler sampler;
    
    public void SpawnItemInArena()
    {
        SpawnTank(sampler.GetNavMeshPointInBound(), Quaternion.identity, false);
    }

    private void SpawnTank(Vector3 spawnPos, Quaternion spawnRot, bool isPlayer)
    {
        if (!isPlayer)
        {
            var randomItemPrefab = itemPrefabs[UnityEngine.Random.Range(0, itemPrefabs.Count)];
            Instantiate(randomItemPrefab, spawnPos, spawnRot);
        }
    }
}
