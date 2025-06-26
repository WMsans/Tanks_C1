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

                var dodgeRight = Vector3.Cross(bulletVelocityNorm, Vector3.up).normalized;
                var dodgeLeft = -dodgeRight;

                var vectorToEnemy = transform.position - incomingBullet.transform.position;

                var side = Vector3.Dot(dodgeRight, vectorToEnemy);

                var isPathRightClear = !Physics.Raycast(transform.position, dodgeRight, 3f, obstacleLayer);
                var isPathLeftClear = !Physics.Raycast(transform.position, dodgeLeft, 3f, obstacleLayer);

                bool dodged = false;
                if (side > 0) 
                {
                    if (isPathRightClear)
                    {
                        _dodgeDirection = dodgeRight;
                        dodged = true;
                    }
                    else if (isPathLeftClear) 
                    {
                        _dodgeDirection = dodgeLeft;
                        dodged = true;
                    }
                }
                else 
                {
                    if (isPathLeftClear)
                    {
                        _dodgeDirection = dodgeLeft;
                        dodged = true;
                    }
                    else if (isPathRightClear) 
                    {
                        _dodgeDirection = dodgeRight;
                        dodged = true;
                    }
                }

                if (!dodged)
                {
                    _dodgeDirection = (transform.position - incomingBullet.transform.position).normalized;
                }

            }
            else
            {

                Owner.ChangeState(followState);
                return;
            }
        }
        else
        {

            Owner.ChangeState(followState);
            return;
        }
    }

    public override void OnUpdateState()
    {

        if (!IsBulletClose())
        {
            Owner.ChangeState(followState);
            return;
        }

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

        HandleRotation(rotSpeed, rotAccel, _rotVal);
        HandlePosition(dodgeSpeed, moveAccel, 1f);
    }

    private GameObject FindIncomingBullet()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius, bulletLayer, QueryTriggerInteraction.Ignore);
        GameObject closestThreat = null;
        var closestDistSqr = float.MaxValue;

        foreach (var bulletCollider in colliders)
        {
            var bulletRb = bulletCollider.GetComponent<Rigidbody>();

            if (bulletRb == null || bulletRb.linearVelocity.sqrMagnitude < 0.1f) continue;

            var toEnemy = (transform.position - bulletCollider.transform.position).normalized;

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

    private bool IsBulletClose()
    {
        return Physics.CheckSphere(transform.position, sightRadius, bulletLayer, QueryTriggerInteraction.Ignore);
    }
}