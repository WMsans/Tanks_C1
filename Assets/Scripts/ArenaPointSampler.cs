using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ArenaPointSampler : MonoSingleton<ArenaPointSampler>
{
    [SerializeField] private int retryCnt;
    [SerializeField] private float minDistanceDiff;
    [SerializeField] Collider arena;
    [SerializeField] private LayerMask layerToAvoid;
    public Vector3 GetRandomPointInBound()
    {
        var bounds = arena.bounds;
        var rect = new Vector3
        {
            x = Random.Range(bounds.min.x, bounds.max.x),
            y = Random.Range(bounds.min.y, bounds.max.y),
            z = Random.Range(bounds.min.z, bounds.max.z)
        };
        return rect;
    }

    public Vector3 GetNavMeshPointInBound()
    {
        for(var i = 0; i < retryCnt ; i++)
        {
            if (NavMesh.SamplePosition(GetRandomPointInBound(), out var hitInfo, 10f, NavMesh.AllAreas))
            {
                if(Physics.OverlapSphere(hitInfo.position, minDistanceDiff, layerToAvoid).Length == 0)
                {
                    return hitInfo.position;
                }
            }
        }
        Debug.LogError("Couldn't find a valid sample point");
        return Vector3.zero;
    }
}
