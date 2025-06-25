using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBaseState : BaseState
{
    protected Rigidbody rb { get; private set; }

    private float _currentRotationSpeed;
    public override void OnEnterState()
    {
        rb = Owner.GetComponent<Rigidbody>();
    }

    protected void HandlePosition(float moveSpeed, float moveAccel, float input)
    {
        rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, transform.forward * input * moveSpeed, moveAccel);
    }

    protected void HandleRotation(float rotSpeed, float rotAccel, float input)
    {
        _currentRotationSpeed = Mathf.MoveTowards(_currentRotationSpeed, rotSpeed * input, rotAccel);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, _currentRotationSpeed, 0f));
    }

    protected void HandleAiming(Transform topRoot, Vector3 lookPosition)
    {
        topRoot.rotation = Quaternion.LookRotation(topRoot.position - lookPosition);
    }
}
