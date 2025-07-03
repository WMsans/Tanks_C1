using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserAttackIndicator : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    [Tooltip("The maximum distance the indicator line will travel.")]
    [SerializeField] private float maxDistance = 100f;
    [Tooltip("Layers that the indicator line will collide with.")]
    [SerializeField] private LayerMask obstacleLayer;
    [Tooltip("The maximum number of times the laser can bounce.")]
    [SerializeField] private int maxBounceTime = 5;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        SimulateLaser(transform.position, transform.forward);
    }

    private void SimulateLaser(Vector3 startPosition, Vector3 initialDirection)
    {
        _lineRenderer.positionCount = 0;

        var points = new List<Vector3> { startPosition };
        var currentPosition = startPosition;
        var currentDirection = initialDirection;
        var bounceCount = 0;

        while (bounceCount < maxBounceTime)
        {
            if (Physics.Raycast(currentPosition, currentDirection, out var hit, maxDistance, obstacleLayer))
            {
                if (Vector3.Distance(hit.point, currentPosition) < .5f)
                {
                    points.Add(currentPosition + currentDirection * maxDistance);
                    break;
                }
                points.Add(hit.point);
                currentPosition = hit.point;
                currentDirection = Vector3.Reflect(currentDirection, hit.normal);
                bounceCount++;
            }
            else
            {
                points.Add(currentPosition + currentDirection * maxDistance);
                break;
            }
        }

        _lineRenderer.positionCount = points.Count;
        _lineRenderer.SetPositions(points.ToArray());
    }
}