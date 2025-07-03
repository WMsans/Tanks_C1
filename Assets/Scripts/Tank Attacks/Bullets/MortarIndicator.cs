using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MortarIndicator : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public LayerMask groundMask;
    public int lineSegments = 50;
    public float fireHeight;
    public float explosionRadius;
    public GameObject landingCircle;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            var targetPosition = hitInfo.point;
            landingCircle.SetActive(true);
            landingCircle.transform.position = targetPosition + Vector3.up * 0.1f;
            landingCircle.transform.localScale = Vector3.one * explosionRadius * 2;
            RenderParabola(transform.position, targetPosition);
        }
        else
        {
            landingCircle.SetActive(false);
            lineRenderer.positionCount = 0;
        }
    }

    void RenderParabola(Vector3 start, Vector3 end)
    {
        var gravity = Physics.gravity.y;
        var initialYVelocity = Mathf.Sqrt(fireHeight * -2f * gravity);
        var displacement = end - start;
        var time = (initialYVelocity + Mathf.Sqrt(Mathf.Pow(initialYVelocity, 2) + 2f * gravity * displacement.y)) / -gravity;
        var horizontalVelocity = new Vector3(displacement.x / time, 0, displacement.z / time);

        lineRenderer.positionCount = lineSegments + 1;
        var parabolaPoints = new Vector3[lineSegments + 1];

        for (int i = 0; i <= lineSegments; i++)
        {
            float t = (float)i / lineSegments;
            float flightTime = t * time;
            Vector3 point = start + horizontalVelocity * flightTime + Vector3.up * (initialYVelocity * flightTime + 0.5f * gravity * Mathf.Pow(flightTime, 2));
            parabolaPoints[i] = point;
        }

        lineRenderer.SetPositions(parabolaPoints);
    }
}