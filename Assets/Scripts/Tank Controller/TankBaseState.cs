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
    private TankAttackController _attackController;
    public override void OnEnterState()
    {
        rb = Owner.GetComponentInChildren<Rigidbody>();
        _attackController = Owner.GetComponentInChildren<TankAttackController>();
        tankAttack = _attackController.TankAttack;
    }

    protected void Update()
    {
        inputInfo = InputSystemManager.Instance.CurrentInputInfo;
        if(_attackController) tankAttack = _attackController.TankAttack;
    }
}
