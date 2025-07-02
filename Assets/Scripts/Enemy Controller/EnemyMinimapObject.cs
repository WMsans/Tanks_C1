using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMinimapObject : MonoBehaviour, IMinimapObject
{
    public MinimapTypeEnum MinimapType => MinimapTypeEnum.Enemy;
    public Action onDestroyed { get; set; }
    public bool RenderAsRealScale => false;

    private void OnEnable()
    {
        Minimap.Instance.AddToObjectHashSet(this);
    }

    private void OnDisable()
    {
        onDestroyed?.Invoke();
    }
}
