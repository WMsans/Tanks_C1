using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNormalState : EnemyBaseState
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotSpeed;
    [SerializeField] private float moveAccel;
    [SerializeField] private float rotAccel;
    [SerializeField] private Transform topRoot;
    private Transform _player;
    private NavMeshPath _path;
    private float _rotVal;
    private float _moveVal;
    public override void OnEnterState()
    {
        base.OnEnterState();
        _path = new();
        _player = GameObject.FindWithTag("Player").transform;
    }

    public override void OnUpdateState()
    {
        if (NavMesh.CalculatePath(rb.position, _player.position, NavMesh.AllAreas, _path))
        {
            float sign = Vector3.Dot(transform.right, _path.corners[1] - transform.position);
            if (sign < -0.01f)
            {
                _rotVal = -1f;
            }
            else if (sign > 0.01f)
            {
                _rotVal = 1f;
            }
            else
            {
                _rotVal = 0f;
            }

            _moveVal = Mathf.Abs(sign) < .75f ? 1f : 0f;
        }
    }

    public override void OnFixedUpdateState()
    {
        HandlePosition(moveSpeed, moveAccel, _moveVal);
        HandleRotation(rotSpeed, rotAccel, _rotVal);
        HandleAiming(topRoot, _player.position);
    }
}
