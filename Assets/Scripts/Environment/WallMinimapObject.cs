using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMinimapObject : MonoBehaviour, IMinimapObject
{
    public MinimapTypeEnum MinimapType => MinimapTypeEnum.Wall;
    public Action onDestroyed { get; set; }
    public bool RenderAsRealScale => true;

    private void OnEnable()
    {
        StartCoroutine(InitializeCoroutine());
    }

    private IEnumerator InitializeCoroutine()
    {
        yield return new WaitForEndOfFrame();
        Minimap.Instance.AddToObjectHashSet(this);
    }

    private void OnDisable()
    {
        onDestroyed?.Invoke();
    }
}
