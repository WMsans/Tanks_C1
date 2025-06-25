using UnityEngine;

public class EnemyDodgeState : EnemyBaseState
{
    [Header("Dependencies")]
    [SerializeField] private EnemyFollowPlayerState followState;

    [Header("Dodge Parameters")]
    [SerializeField] private LayerMask bulletLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float dodgeSpeed = 8f;
    [SerializeField] private float sightRadius = 15f;

    [Header("Movement")]
    [SerializeField] private float rotSpeed = 5f;
    [SerializeField] private float rotAccel = 0.2f;
    [SerializeField] private float moveAccel = 0.2f;

    private Vector3 _dodgeDirection;
    private float _rotVal;

    public override void OnEnterState()
    {
        base.OnEnterState();

        var incomingBullet = FindIncomingBullet();
        if (incomingBullet != null)
        {
            var bulletRb = incomingBullet.GetComponent<Rigidbody>();
            if (bulletRb != null && bulletRb.linearVelocity.sqrMagnitude > 0.1f)
            {
                var bulletVelocityNorm = bulletRb.linearVelocity.normalized;

                // --- Start of Updated Logic ---

                // 1. Calculate the two perpendicular dodge directions (left and right relative to the bullet's velocity)
                var dodgeRight = Vector3.Cross(bulletVelocityNorm, Vector3.up).normalized;
                var dodgeLeft = -dodgeRight;

                // 2. Determine if the enemy is on the left or right side of the bullet's path
                var vectorToEnemy = transform.position - incomingBullet.transform.position;
                // By getting the dot product with the "right" vector, we can determine the side.
                // A positive result means the enemy is more to the right, negative means more to the left.
                var side = Vector3.Dot(dodgeRight, vectorToEnemy);

                // 3. Check if the paths in those directions are clear of obstacles
                var isPathRightClear = !Physics.Raycast(transform.position, dodgeRight, 3f, obstacleLayer);
                var isPathLeftClear = !Physics.Raycast(transform.position, dodgeLeft, 3f, obstacleLayer);

                // 4. Decide on the best dodge direction
                bool dodged = false;
                if (side > 0) // The enemy is on the right side of the bullet
                {
                    if (isPathRightClear)
                    {
                        _dodgeDirection = dodgeRight;
                        dodged = true;
                    }
                    else if (isPathLeftClear) // Try the other side if the preferred one is blocked
                    {
                        _dodgeDirection = dodgeLeft;
                        dodged = true;
                    }
                }
                else // The enemy is on the left side or directly in front
                {
                    if (isPathLeftClear)
                    {
                        _dodgeDirection = dodgeLeft;
                        dodged = true;
                    }
                    else if (isPathRightClear) // Try the other side if the preferred one is blocked
                    {
                        _dodgeDirection = dodgeRight;
                        dodged = true;
                    }
                }

                // If both paths are blocked, use the fallback behavior (dodge directly away from bullet)
                if (!dodged)
                {
                    _dodgeDirection = (transform.position - incomingBullet.transform.position).normalized;
                }

                // --- End of Updated Logic ---
            }
            else
            {
                // Fallback if the bullet has no Rigidbody or is not moving
                Owner.ChangeState(followState);
                return;
            }
        }
        else
        {
            // If no bullet is found, no need to dodge
            Owner.ChangeState(followState);
            return;
        }
    }

    public override void OnUpdateState()
    {
        // If no more bullets are nearby, stop dodging
        if (!IsBulletClose())
        {
            Owner.ChangeState(followState);
            return;
        }

        // Determine rotation direction based on the chosen dodge direction
        var sign = Vector3.Dot(transform.right, _dodgeDirection);

        _rotVal = sign switch
        {
            < -0.01f => -1f,
            > 0.01f => 1f,
            _ => 0f
        };
    }

    public override void OnFixedUpdateState()
    {
        // Apply rotation and movement
        HandleRotation(rotSpeed, rotAccel, _rotVal);
        HandlePosition(dodgeSpeed, moveAccel, 1f);
    }

    /// <summary>
    /// Finds the most threatening incoming bullet within the sight radius.
    /// </summary>
    private GameObject FindIncomingBullet()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius, bulletLayer, QueryTriggerInteraction.Ignore);
        GameObject closestThreat = null;
        var closestDistSqr = float.MaxValue;

        foreach (var bulletCollider in colliders)
        {
            var bulletRb = bulletCollider.GetComponent<Rigidbody>();
            // Ensure the bullet has a rigidbody and is actually moving
            if (bulletRb == null || bulletRb.linearVelocity.sqrMagnitude < 0.1f) continue;

            var toEnemy = (transform.position - bulletCollider.transform.position).normalized;
            // Check if the bullet is generally heading towards the enemy
            if (Vector3.Dot(bulletRb.linearVelocity.normalized, toEnemy) > 0.5f)
            {
                var distSqr = (bulletCollider.transform.position - transform.position).sqrMagnitude;
                if (distSqr < closestDistSqr)
                {
                    closestDistSqr = distSqr;
                    closestThreat = bulletCollider.gameObject;
                }
            }
        }
        return closestThreat;
    }

    /// <summary>
    /// Checks if any bullets are within the sight radius.
    /// </summary>
    private bool IsBulletClose()
    {
        return Physics.CheckSphere(transform.position, sightRadius, bulletLayer, QueryTriggerInteraction.Ignore);
    }
}