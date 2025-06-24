using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TankBaseState : BaseState
{
    protected Rigidbody rb { get; private set; }
    protected InputSystemManager.InputInfo inputInfo { get; private set; }

    protected void Awake()
    {
        rb = Owner.GetComponent<Rigidbody>();
    }

    protected void Update()
    {
        inputInfo = InputSystemManager.Instance.CurrentInputInfo;
    }
}
