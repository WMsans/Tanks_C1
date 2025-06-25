using UnityEngine;
using System.Collections.Generic;

public class EnemyDodgeState : EnemyBaseState
{
    [Header("Dependencies")]
    [SerializeField] private EnemyFollowPlayerState followState;

    [Header("Dodge Parameters")]
    [SerializeField] private LayerMask bulletLayer;
    [SerializeField] private LayerMask obstacleLayer; // Assign walls, etc. to this layer
    [SerializeField] private float dodgeSpeed = 8f;
    [SerializeField] private float dodgePredictionTime = 0.5f; // How far in the future to predict bullet positions
    [SerializeField] private float dodgeSafetyMargin = 1.5f; // How far to stay away from a predicted bullet path
    [SerializeField] private int dodgeDirectionSamples = 16; // Number of directions to check for a safe spot

    [Header("Movement")]
    [SerializeField] private float rotSpeed = 5f;
    [SerializeField] private float rotAccel = 0.2f;
    [SerializeField] private float moveAccel = 0.2f;

    private Vector3 _dodgeTargetPosition;
    private float _rotVal;

    public override void OnEnterState()
    {
        base.OnEnterState();

        var incomingBullet = FindIncomingBullet();
        if (incomingBullet != null)
        {
            if (FindSafeDodgePosition(incomingBullet, out _dodgeTargetPosition))
            {
                Debug.Log("Found safe dodge position. Dodging!");
            }
            else
            {
                // Fallback: If no perfectly safe spot is found, dodge perpendicular to the bullet's path
                var bulletRb = incomingBullet.GetComponent<Rigidbody>();
                Vector3 perpendicularDodge = Vector3.Cross(bulletRb.linearVelocity.normalized, Vector3.up);
                
                // Check which perpendicular direction is clearer of obstacles
                if (Physics.Raycast(transform.position, perpendicularDodge, 2f, obstacleLayer))
                {
                    perpendicularDodge = -perpendicularDodge; // Try the other way
                }
                _dodgeTargetPosition = transform.position + perpendicularDodge * 3f; // Dodge 3 units
                Debug.LogWarning("No ideal safe spot found. Executing emergency dodge.");
            }
        }
        else
        {
            // No immediate threat, return to follow state
            Owner.ChangeState(followState);
            return;
        }
    }

    public override void OnUpdateState()
    {
        // If we have reached our dodge position, or if the danger has passed, switch state
        if (Vector3.Distance(transform.position, _dodgeTargetPosition) < 0.5f || !IsBulletClose())
        {
            Owner.ChangeState(followState);
            return;
        }

        // Determine rotation direction towards the dodge target
        Vector3 directionToTarget = (_dodgeTargetPosition - transform.position).normalized;
        float sign = Vector3.Dot(transform.right, directionToTarget);

        _rotVal = sign switch
        {
            < -0.01f => -1f,
            > 0.01f => 1f,
            _ => 0f
        };
    }

    public override void OnFixedUpdateState()
    {
        HandleRotation(rotSpeed, rotAccel, _rotVal);

        // Move forward as we rotate towards the target direction
        HandlePosition(dodgeSpeed, moveAccel, 1f);
    }

    private GameObject FindIncomingBullet()
    {
        var colliders = Physics.OverlapSphere(transform.position, 15f, bulletLayer, QueryTriggerInteraction.Ignore);
        GameObject closestThreat = null;
        float closestTimeOfImpact = float.MaxValue;

        foreach (var bulletCollider in colliders)
        {
            var bulletRb = bulletCollider.GetComponent<Rigidbody>();
            if (bulletRb == null || bulletRb.linearVelocity.sqrMagnitude < 0.1f) continue;

            Vector3 bulletVel = bulletRb.linearVelocity;
            Vector3 relativePos = transform.position - bulletCollider.transform.position;
            
            // Only consider bullets moving towards us
            if (Vector3.Dot(relativePos, bulletVel) < 0) continue;

            // Calculate time of impact (a simplified approach)
            float timeToImpact = relativePos.magnitude / bulletVel.magnitude;

            if (timeToImpact < closestTimeOfImpact)
            {
                closestTimeOfImpact = timeToImpact;
                closestThreat = bulletCollider.gameObject;
            }
        }
        return closestThreat;
    }

    private bool FindSafeDodgePosition(GameObject bullet, out Vector3 safePosition)
    {
        var bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb == null)
        {
            safePosition = Vector3.zero;
            return false;
        }
        
        // Sample points in a circle around the enemy and evaluate their safety
        for (int i = 0; i < dodgeDirectionSamples; i++)
        {
            float angle = i * (360f / dodgeDirectionSamples);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            float distance = dodgeSpeed * dodgePredictionTime; // Dodge distance based on speed and prediction
            Vector3 potentialPosition = transform.position + dir * distance;

            // 1. Check if the path to the position is clear of obstacles
            if (Physics.Raycast(transform.position, dir, distance, obstacleLayer))
            {
                continue; // Path is blocked
            }

            // 2. Predict the bullet's future position
            Vector3 bulletFuturePos = bullet.transform.position + bulletRb.linearVelocity * dodgePredictionTime;

            // 3. Check if our potential position is far enough from the bullet's future position
            if (Vector3.Distance(potentialPosition, bulletFuturePos) > dodgeSafetyMargin)
            {
                safePosition = potentialPosition;
                return true;
            }
        }

        safePosition = Vector3.zero;
        return false;
    }

    private bool IsBulletClose()
    {
        // This remains a good general check for whether to even consider dodging
        return Physics.CheckSphere(transform.position, 15f, bulletLayer, QueryTriggerInteraction.Ignore);
    }
}