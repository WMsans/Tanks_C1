using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineRunner : MonoBehaviour
{
    [SerializeField]private BaseState initialState;
    public BaseState CurrentState { get; private set; }
    public void ChangeState(BaseState next)
    {
        CurrentState?.OnExitState();
        CurrentState = next;
        CurrentState.SetOwner(this);
        CurrentState.OnEnterState();
    }

    private void Start()
    {
        ChangeState(initialState);
    }

    private void Update()
    {
        CurrentState.OnUpdateState();
    }

    private void FixedUpdate()
    {
        CurrentState.OnFixedUpdateState();
    }

    private void LateUpdate()
    {
        CurrentState.OnLateUpdateState();
    }
}
