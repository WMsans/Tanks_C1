using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerRoot : MonoSingleton<ManagerRoot>
{
    protected override void Awake()
    {
        base.Awake();
        if(!gameObject) return;
        DontDestroyOnLoad(gameObject);
    }
}
