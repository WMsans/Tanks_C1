using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class handles the tank's rotation and movement.
/// </summary>
public class TankMove : MonoBehaviour
{
    // [SerializeField] allows this private variable to be visible in the Inspector and supports hot-reloading during runtime,
    // making it convenient to tune game behavior without recompiling the code.
    [SerializeField] float moveSpeed = 1.0f; // move speed
    [SerializeField] float rotSpeed = 1.0f; // rotation speed
    [SerializeField] private float moveAccel;
    [SerializeField] private float rotAccel;

    float moveVal; // Movement value received from hardware input
    float rotVal; // Rotation value received from hardware input
    Rigidbody rb; // rigidbody of this gameobject

    private InputAction moveTank;
    private InputAction rotTank;

    private void Start()
    {
        moveVal = 0f;
        rotVal = 0f;
        rb = GetComponent<Rigidbody>();

        moveTank = InputSystem.actions.FindAction("TankMove");
        rotTank = InputSystem.actions.FindAction("RotMove");
    }

    void FixedUpdate()
    {
        UpdateRotation();
        UpdatePosition();
    }

    private void UpdateRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, rotVal * rotSpeed, 0f));
    }

    public void OnTankRotate(InputValue value)
    {
        rotVal = value.Get<float>();
    }

    private void UpdatePosition()
    {
        rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, transform.forward * moveVal * moveSpeed, moveAccel);
    }

    public void OnTankMove(InputValue value)
    {
        moveVal = value.Get<float>();
    }
}