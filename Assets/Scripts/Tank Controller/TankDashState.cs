using TrailsFX;
using UnityEngine;

public class TankDashState : TankBaseState
{
    [Header("Attack")] 
    [SerializeField] private Transform topRoot;
    [SerializeField] private Transform shootPoint;
    [Header("Effect")] 
    [SerializeField] private TrailEffect trail;

    private float _currentRotationSpeed = 0f;
    private float _lastAttackTime;
    public override void OnEnterState()
    {
        base.OnEnterState();
        trail.active = true;
    }

    public override void OnExitState()
    {
        base.OnExitState();
        trail.active = false;
    }

    public override void OnFixedUpdateState()
    {
        HandlePosition();
        HandleRotation();
        HandleAiming();
    }
    
    private void HandlePosition()
    {
        // Extract the forward and sideways components of the tank's velocity
        Vector3 forwardVelocity = Vector3.Project(rb.linearVelocity, transform.forward);
        Vector3 sidewaysVelocity = rb.linearVelocity - forwardVelocity;

        // Accelerate the tank in its forward direction
        forwardVelocity += transform.forward * config.dashAccel * Time.fixedDeltaTime;

        // Ensure the forward speed does not surpass the maximum dash speed
        if (forwardVelocity.magnitude > config.dashMoveSpeed)
        {
            forwardVelocity = forwardVelocity.normalized * config.dashMoveSpeed;
        }

        // Apply a deceleration to the sideways velocity to create a 'drag' effect
        sidewaysVelocity = Vector3.MoveTowards(sidewaysVelocity, Vector3.zero, config.dashDecel * Time.fixedDeltaTime);

        // Recombine the velocity components and apply to the rigidbody
        rb.linearVelocity = forwardVelocity + sidewaysVelocity;
    }

    private void HandleRotation()
    {
        _currentRotationSpeed = Mathf.MoveTowards(_currentRotationSpeed, config.rotSpeed * inputInfo.RotationAxis, config.rotAccel);
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
        if (!InputSystemManager.Instance.CurrentInputInfo.DashHold)
        {
            Owner.ChangeState(GetComponent<TankNormalState>());
        }
    }
    private void HandleAttack()
    {
        if (InputSystemManager.Instance.CurrentInputInfo.AttackDown)
        {
            if (CanShoot())
            {
                _lastAttackTime = Time.time;
                tankAttack.OnAttack(shootPoint);
            }
        }
    }
    private bool CanShoot()
    {
        return Time.time - _lastAttackTime > tankAttack.CoolDown;
    }
}