using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TankBaseState : BaseState
{
    [SerializeField] protected TankConfig config;
    protected Rigidbody rb { get; private set; }
    protected InputSystemManager.InputInfo inputInfo { get; private set; }
    protected ITankAttack tankAttack { get; private set; }

    public override void OnEnterState()
    {
        rb = Owner.GetComponent<Rigidbody>();
        tankAttack = Owner.GetComponent<TankAttackController>().TankAttack;
    }

    protected void Update()
    {
        inputInfo = InputSystemManager.Instance.CurrentInputInfo;
    }
}
