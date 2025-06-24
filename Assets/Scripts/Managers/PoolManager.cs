using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages object pooling by instantiating all necessary pooled objects and supplying them to requesting components.
/// This manager does not need to persist across scenes.
/// </summary>
public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    // Check if this object is the only instance in the scene. (Singleton)
    private void Awake()
    {
        // if the current instance hasn't been assigned, that means this object is the first instance of this class.
        if (instance == null)
        {
            // Set the instance to this object.
            instance = this;
        }
        else
        {
            // if the instance already exist and it's not this object
            if (instance != this)
            {
                // destroy this one
                Destroy(gameObject);
                return;
            }
        }
    }

    private Dictionary<GameObject, List<GameObject>> _pool = new();
    private Dictionary<GameObject, int> _poolLength = new();

    /// <summary>
    /// Get an object from the pool if one is available.
    /// </summary>
    /// <returns>Return a bullet if one is available; otherwise, return null.</returns>
    public GameObject GetPooledObject(GameObject prefab)
    {
        if (_pool.TryGetValue(prefab, out var objectList))
        {
            var availableObject = objectList.FirstOrDefault(x => !x.activeSelf);
            if (!availableObject)
            {
                FillNewPrefab(prefab, objectList, _poolLength[prefab] - 1);
                availableObject = InstantiateNewPrefab(prefab, objectList);
                _poolLength[prefab] *= 2;
            }
            return availableObject;
        }

        objectList = new List<GameObject>();
        _pool.Add(prefab, objectList);
        _poolLength.Add(prefab, 1);
        return InstantiateNewPrefab(prefab, objectList);
    }

    private GameObject InstantiateNewPrefab(GameObject prefab, List<GameObject> objectList)
    {
        var obj = Instantiate(prefab);
        obj.SetActive(false);
        objectList.Add(obj);
        return obj;
    }

    private void FillNewPrefab(GameObject prefab, List<GameObject> objectList, int count)
    {
        for (int i = 1; i <= count; i++)
        {
            InstantiateNewPrefab(prefab, objectList);
        }
    }
}
