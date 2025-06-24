using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseState : BaseState
{
    protected Rigidbody rb { get; private set; }
    protected void Awake()
    {
        rb = Owner.GetComponent<Rigidbody>();
    }
}
