using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMinimapObject : MonoBehaviour, IMinimapObject
{
    [SerializeField] private MinimapTypeEnum type;
    public MinimapTypeEnum MinimapType => type;
    public Action onDestroyed { get; set; }
    public bool RenderAsRealScale => false;

    private void OnEnable()
    {
        Minimap.Instance.AddToObjectHashSet(this);
    }
    private void OnDisable()
    {
        onDestroyed.Invoke();
    }
}
