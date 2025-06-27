using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LasorBulletBehaviour : MonoBehaviour, IPoolable
{
    [Header("Laser Settings")]
    [SerializeField] private int maxBounceTime = 5;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask harmableLayer;
    [SerializeField] private LayerMask obstacleLayer; 
    [SerializeField] private float lasorTime;
    [SerializeField] private Transform ownerPos;
    private GameObject _owner;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        if (gameObject.activeInHierarchy)
        {
            var cols = Physics.OverlapSphere(ownerPos.position, .1f, harmableLayer, QueryTriggerInteraction.UseGlobal);
            if (cols.Length > 0) _owner = cols[0].transform.root.gameObject;
            StartCoroutine(DestroySelfCoroutine());
        }
    }

    private void Update()
    {
        SimulateLaser(transform.position, transform.forward);
    }

    private void SimulateLaser(Vector3 startPosition, Vector3 initialDirection)
    {
        InitializeVariables();

        var points = new List<Vector3> { startPosition };
        var currentPosition = startPosition;
        var currentDirection = initialDirection;
        var bounceCount = 0;

        CheckForHarmableObjects(startPosition, initialDirection);

        while (bounceCount < maxBounceTime)
        {

            if (Physics.Raycast(currentPosition, currentDirection, out var hit, maxDistance, obstacleLayer))
            {
                points.Add(hit.point);
                currentPosition = hit.point;
                currentDirection = Vector3.Reflect(currentDirection, hit.normal);
                bounceCount++;

                CheckForHarmableObjects(currentPosition, currentDirection);
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

    private void CheckForHarmableObjects(Vector3 startPosition, Vector3 direction)
    {
        var hits = Physics.RaycastAll(startPosition, direction, maxDistance, harmableLayer);
        foreach (var hit in hits)
        {
            if(hit.collider.transform.root.gameObject != _owner)
            OnHit(hit.collider);
        }
    }

    public void InitializeVariables()
    {
        _lineRenderer.positionCount = 0;
    }

    private IEnumerator DestroySelfCoroutine()
    {
        yield return new WaitForSeconds(lasorTime);
        gameObject.SetActive(false);
    }

    private void OnHit(Collider other)
    {
        var hitDamage = Mathf.Infinity;
        if (other.gameObject.transform.root.TryGetComponent<IDamageable>(out var damageable))
        {

            damageable.OnHit(hitDamage);
        }
    }

    private void OnExplode()
    {
        gameObject.SetActive(false);
        return;
    }
}