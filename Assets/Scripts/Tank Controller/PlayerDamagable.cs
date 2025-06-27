using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamagable : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 1;
    [SerializeField] private Slider hpBar;
    private float maxHp;
    public float Health { get => health; set => health = value; }

    private void Start()
    {
        maxHp = health;
    }

    private void Update()
    {
        hpBar.value = Health / maxHp;
        Debug.Log(Health + " " + maxHp);
    }

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
        hpBar.value = 0;
        gameObject.SetActive(false);
        return;
    }
}
