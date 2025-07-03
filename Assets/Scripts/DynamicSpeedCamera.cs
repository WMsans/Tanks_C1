using Unity.Cinemachine;
using UnityEngine;

public class DynamicSpeedCamera : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("The Rigidbody of the object to track the speed of.")]
    public Rigidbody targetRigidbody;

    [Header("Cinemachine Settings")]
    [Tooltip("The Cinemachine Virtual Camera to control.")]
    public CinemachineCamera virtualCamera;

    [Header("FOV Settings")]
    [Tooltip("The minimum Field of View when the player is stationary.")]
    [SerializeField] private float minFov = 40f;
    [Tooltip("The maximum Field of View at maximum speed.")]
    [SerializeField] private float maxFov = 60f;

    [Header("Distance Settings")]
    [Tooltip("The minimum camera distance when the player is stationary.")]
    [SerializeField] private float minDistance = 10f;
    [Tooltip("The maximum camera distance at maximum speed.")]
    [SerializeField] private float maxDistance = 20f;

    [Header("Speed Settings")]
    [Tooltip("The speed at which the camera effects will be at their maximum.")]
    [SerializeField] private float maxSpeed = 20f;

    [Header("Smoothing")]
    [Tooltip("How quickly the camera adjusts to speed changes. Higher values are faster.")]
    [SerializeField] private float smoothing = 5f;

    private CinemachineFollow _framingTransposer;
    private Vector3 _startFollowOffset;

    void Start()
    {
        if (virtualCamera != null)
        {
            // Get the Framing Transposer component from the virtual camera
            _framingTransposer = virtualCamera.GetComponent<CinemachineFollow>();
            _startFollowOffset = _framingTransposer.FollowOffset.normalized;
        }
        else
        {
            Debug.LogError("Cinemachine Virtual Camera is not assigned.");
        }

        if (targetRigidbody == null)
        {
            Debug.LogError("Target Rigidbody is not assigned.");
        }
    }

    void Update()
    {
        if (targetRigidbody == null || virtualCamera == null || _framingTransposer == null)
        {
            return;
        }

        // Get the current speed of the Rigidbody
        float currentSpeed = targetRigidbody.linearVelocity.magnitude;

        // Calculate the percentage of max speed
        float speedPercentage = Mathf.Clamp01(currentSpeed / maxSpeed);

        // Calculate the target FOV and distance based on the speed percentage
        float targetFov = Mathf.Lerp(minFov, maxFov, speedPercentage);
        float targetDistance = Mathf.Lerp(minDistance, maxDistance, speedPercentage);

        // Smoothly interpolate the camera's current FOV and distance to the target values
        virtualCamera.Lens.FieldOfView = Mathf.Lerp(virtualCamera.Lens.FieldOfView, targetFov, Time.deltaTime * smoothing);
        _framingTransposer.FollowOffset = Vector3.Lerp(_framingTransposer.FollowOffset, _startFollowOffset * targetDistance, Time.deltaTime * smoothing);
    }
}