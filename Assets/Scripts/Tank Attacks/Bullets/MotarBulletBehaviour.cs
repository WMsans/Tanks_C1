using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotarBulletBehaviour : MonoBehaviour, IPoolable
{
    private Rigidbody rb;
    public int damage = 2;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask harmableLayer;
    [SerializeField] private float bulletTime = 10f;
    [SerializeField] private float explosionRadius;
    private Coroutine _destroySelfCoroutine;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        _destroySelfCoroutine = StartCoroutine(DestroySelfCoroutine());
    }
    private IEnumerator DestroySelfCoroutine()
    {
        yield return new WaitForSeconds(bulletTime);
        gameObject.SetActive(false);
    }
    void OnCollisionEnter(Collision other)
    {
        if (((1 << other.gameObject.layer) & harmableLayer) != 0)
        {
            HandleExplosion();
        }else if (((1 << other.gameObject.layer) & wallLayer) != 0)
        {
            HandleExplosion();
        }
    }

    private void HandleExplosion()
    {
        var cols = Physics.OverlapSphere(rb.position, explosionRadius, wallLayer | harmableLayer);
        var attackedObjects = new List<GameObject>();
        foreach (var other in cols)
        {
            if(attackedObjects.Contains(other.transform.root.gameObject)) continue;
            if (other.transform.root.gameObject.TryGetComponent<IDamageable>(out var damageable))
            {
                attackedObjects.Add(other.transform.root.gameObject);
                damageable.OnHit(damage);
            }
        }
        EffectManager.instance.PlayExplosion(transform.position);
        StopCoroutine(_destroySelfCoroutine);
        gameObject.SetActive(false);
    }
    public void InitializeVariables()
    {
        rb.linearVelocity = rb.angularVelocity = Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
