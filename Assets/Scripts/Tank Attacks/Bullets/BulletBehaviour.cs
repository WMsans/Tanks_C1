using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Defines the behavior of a bullet.
/// The bullet bounces off walls and explodes on player or enemy tanks.
/// It deals damage by interacting with objects that implement the IDamageable interface.
/// If all bounces are used, it will also damage a wall on the final impact.
/// </summary>
public class BulletBehaviour : MonoBehaviour, IPoolable
{
    private Rigidbody rb;
    public bool hasBounced = false;
    public int damage = 1;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float bulletTime;
    [FormerlySerializedAs("enemyLayer")] [SerializeField] private LayerMask harmableLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        StartCoroutine(DestroySelfCoroutine());
    }

    /// <summary>
    /// Detect if the object hits something on the Environment (Wall), Player, or Enemy layers. 
    /// Perform OnHit detection, and if it hits a wall, also perform a bounce test.
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter(Collision other)
    {
        if (((1 << other.gameObject.layer) & harmableLayer) != 0)
        {
            HandleHit(other, damage);
        }else if (((1 << other.gameObject.layer) & wallLayer) != 0)
        {
            HandleHit(other, damage);
        }
        
    }

    private void HandleHit(Collision other, float hitDamage)
    {
        // Handle damage
        if (other.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            if (damageable.OnHit(hitDamage))
            {
                OnExplode();
                return;
            }
        }
        // Handle bounce
        if (hasBounced)
        {
            OnExplode();
        }
        hasBounced = true;
    }

    private void OnExplode()
    {
        EffectManager.instance.PlayExplosion(transform.position);
        gameObject.SetActive(false);
        return;
    }

    public void InitializeVariables()
    {
        rb.linearVelocity = rb.angularVelocity = Vector3.zero;
        hasBounced = false;
    }
    private IEnumerator DestroySelfCoroutine()
    {
        yield return new WaitForSeconds(bulletTime);
        gameObject.SetActive(false);
    }
}

