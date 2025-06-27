using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireState : EnemyBaseState
{
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float sightRadius = 5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask bulletLayer;
    [SerializeField] private EnemyFollowPlayerState followState;
    [SerializeField] private EnemyDodgeState dodgeState;
    
    private float nextFireTime;

    public override void OnEnterState()
    {
        base.OnEnterState();
        nextFireTime = Time.time + 1f / fireRate;
        Debug.Log("Firing");
    }

    public override void OnUpdateState()
    {
        if (IsBulletClose())
        {
            Owner.ChangeState(dodgeState);
            return;
        }

        if (Time.time >= nextFireTime)
        {
            if (CanSeePlayer())
            {
                Shoot();
                Owner.ChangeState(followState);
            }
        }
    }

    private bool CanSeePlayer()
    {
        for (int i = 0; i <= 22; i++)
        {
            var angle = -115 + (i * 10);
            var direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            var ray = new Ray(firePoint.position, direction);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, playerLayer | wallLayer))
            {
                if ((playerLayer & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    return true;
                }

                if ((wallLayer & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    var bouncedDirection = Vector3.Reflect(direction, hit.normal);
                    var bouncedRay = new Ray(hit.point, bouncedDirection);

                    if (Physics.Raycast(bouncedRay, out var bouncedHit, Mathf.Infinity, playerLayer | (1 << gameObject.layer)))
                    {
                        if ((playerLayer & (1 << bouncedHit.collider.gameObject.layer)) != 0)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private void Shoot()
    {
        tankAttack.OnAttack(firePoint);
        nextFireTime = Time.time + 1f / fireRate;
    }

    private bool IsBulletClose()
    {
        return Physics.CheckSphere(transform.position, sightRadius, bulletLayer, QueryTriggerInteraction.Ignore);
    }
}