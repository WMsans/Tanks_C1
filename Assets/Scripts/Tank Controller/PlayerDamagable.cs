using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamagable : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 1;

    public float Health { get => health; set => health = value; }
    public bool OnHit(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            OnDeath();
            return true;
        }

        return false;
    }
    private void OnDeath()
    {
        EffectManager.instance.PlayExplosion(transform.position);
        SceneSystemManager.Instance.ChangeSceneOnDelay("2_End", 2f);
        gameObject.SetActive(false);
        return;
    }
}
