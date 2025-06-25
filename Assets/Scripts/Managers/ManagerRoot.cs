using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ManagerRoot : MonoSingleton<ManagerRoot>
{
    protected override void Awake()
    {
        base.Awake();
        if(!gameObject) return;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Random.InitState((int)DateTime.Now.Ticks);
    }
}
