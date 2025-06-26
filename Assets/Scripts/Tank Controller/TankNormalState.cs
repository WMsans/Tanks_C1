using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankNormalState : TankBaseState
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f; 
    [SerializeField] private float rotSpeed = 2.0f; 
    [SerializeField] private float moveAccel;
    [SerializeField] private float rotAccel;
    [Header("Attack")] 
    [SerializeField] private Transform topRoot;

    [SerializeField] private float cooldown;
    private float _currentRotationSpeed = 0f;
    public override void OnFixedUpdateState()
    {
        HandlePosition();
        HandleRotation();
        HandleAiming();
    }
    
    private void HandlePosition()
    {
        rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, transform.forward * inputInfo.MoveAxis * moveSpeed, moveAccel);
    }

    private void HandleRotation()
    {
        _currentRotationSpeed = Mathf.MoveTowards(_currentRotationSpeed, rotSpeed * inputInfo.RotationAxis, rotAccel);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, _currentRotationSpeed, 0f));
    }

    private void HandleAiming()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var groundPlane = new Plane(Vector3.up, topRoot.position);

        if (groundPlane.Raycast(ray, out var distance))
        {
            var targetPoint = ray.GetPoint(distance);
            var directionToFace = targetPoint - topRoot.position;
            directionToFace.y = 0;
            if (directionToFace.sqrMagnitude > 0.1f)
            {
                topRoot.rotation = Quaternion.LookRotation(directionToFace);
            }
        }
    }

    public override void OnUpdateState()
    {
        HandleAttack();
    }
    private void HandleAttack()
    {
        if (InputSystemManager.Instance.CurrentInputInfo.AttackDown)
        {
            if (CanShoot())
            {
                tankAttack.OnAttack();
            }
        }
    }
    private bool CanShoot()
    {
        return Time.time - tankAttack.LastAttackTime > cooldown;
    }
}
