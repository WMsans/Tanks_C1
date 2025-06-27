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
    [SerializeField] private float lasorTime;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Fire(Vector3 startPosition, Vector3 initialDirection)
    {
        InitializeVariables();
        SimulateLaser(startPosition, initialDirection);
    }

    private void SimulateLaser(Vector3 currentPosition, Vector3 currentDirection)
    {
        var points = new List<Vector3> { currentPosition };
        var bounceCount = 0;

        while (bounceCount < maxBounceTime)
        {
            if (Physics.Raycast(currentPosition, currentDirection, out var hit, maxDistance))
            {

                points.Add(hit.point);

                if ((harmableLayer.value & (1 << hit.collider.gameObject.layer)) > 0)
                {

                    OnHit(hit.collider);
                    break; 
                }

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

    public void InitializeVariables()
    {
        _lineRenderer.positionCount = 0;
    }

    private void OnEnable()
    {
        Fire(transform.position, transform.forward);
        StartCoroutine(DestroySelfCoroutine());
    }

    private IEnumerator DestroySelfCoroutine()
    {
        yield return new WaitForSeconds(lasorTime);
        gameObject.SetActive(false);
    }

    private void OnHit(Collider other)
    {
        var hitDamage = Mathf.Infinity;
        if (other.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            if (damageable.OnHit(hitDamage))
            {
                OnExplode();
                return;
            }
        }
    }
    private void OnExplode()
    {
        EffectManager.instance.PlayExplosion(transform.position);
        gameObject.SetActive(false);
        return;
    }
}