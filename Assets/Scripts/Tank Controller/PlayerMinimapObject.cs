using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMinimapObject : MonoBehaviour, IMinimapObject
{
    public MinimapTypeEnum MinimapType => MinimapTypeEnum.Player;
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
