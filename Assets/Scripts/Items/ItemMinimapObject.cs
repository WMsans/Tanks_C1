using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMinimapObject : MonoBehaviour, IMinimapObject
{
    public MinimapTypeEnum MinimapType => MinimapTypeEnum.PickupHealing;
    public Action onDestroyed { get; set; }
    private void OnEnable()
    {
        Minimap.Instance.AddToObjectHashSet(this);
    }
    private void OnDisable()
    {
        onDestroyed.Invoke();
    }
}
