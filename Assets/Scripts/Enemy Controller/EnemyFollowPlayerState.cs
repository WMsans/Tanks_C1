using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollowPlayerState : EnemyBaseState
{
    [SerializeField] private float sightRadius = 5f;
    [SerializeField] private Transform topRoot;
    [SerializeField] private float shootingRange = 10f;
    [SerializeField] private EnemyFireState fireState;
    [SerializeField] private EnemyDodgeState dodgeState;
    [SerializeField] private LayerMask bulletLayer;
    [SerializeField] private float minimumTimeFollowing;
    private Transform _player;
    private NavMeshPath _path;
    private float _rotVal;
    private float _moveVal;
    private float _enterStateTime;
    public override void OnEnterState()
    {
        base.OnEnterState();
        _path = new();
        _player = GameObject.FindWithTag("Player")?.transform;
        _enterStateTime = Time.time;
    }

    public override void OnUpdateState()
    {
        if(!_player) return;
        if (IsBulletClose())
        {
            Owner.ChangeState(dodgeState);
            return;
        }

        if (Vector3.Distance(transform.position, _player.position) <= shootingRange && Time.time - _enterStateTime > minimumTimeFollowing)
        {
            Owner.ChangeState(fireState);
            return;
        }

        if (NavMesh.CalculatePath(rb.position, _player.position, NavMesh.AllAreas, _path))
        {
            if (_path.corners.Length > 1)
            {
                var sign = Vector3.Dot(transform.right, _path.corners[1] - transform.position);
                _rotVal = sign switch
                {
                    < -0.01f => -1f,
                    > 0.01f => 1f,
                    _ => 0f
                };

                _moveVal = Mathf.Abs(sign) < .75f ? 1f : 0f;
            }
        }
    }
    
    private GameObject FindIncomingBullet()
    {
        var colliders = Physics.OverlapSphere(transform.position, 5f, bulletLayer);
        foreach (var bulletCollider in colliders)
        {
            var bulletRb = bulletCollider.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                var direction = bulletRb.linearVelocity.normalized;
                var ray = new Ray(bulletCollider.transform.position, direction);
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        return bulletCollider.gameObject;
                    }
                }
            }
        }
        return null;
    }

    public override void OnFixedUpdateState()
    {
        HandlePosition(config.moveSpeed, config.moveAccel, _moveVal);
        HandleRotation(config.rotSpeed, config.rotAccel, _rotVal);
        HandleAiming(topRoot, _player.position);
    }

    private bool IsBulletClose()
    {
        return Physics.CheckSphere(transform.position, sightRadius, bulletLayer);
    }
}