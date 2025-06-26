using System;
using UnityEngine;

public class TankAttack : MonoBehaviour, ITankAttack
{
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float firePower;
    private float _lastShootTime;
    public float LastAttackTime => _lastShootTime;

    public void OnAttack()
    {
        _lastShootTime = Time.time;
        //var bulletObject = Instantiate(bulletPrefab);
        var bulletObject = PoolManager.instance.GetPooledObject(bulletPrefab);
        bulletObject.transform.position = shootPoint.transform.position;
        bulletObject.transform.rotation = shootPoint.transform.rotation;
        bulletObject.SetActive(true);
        var bulletRb = bulletObject.GetComponent<Rigidbody>();
        bulletRb.AddForce(firePower * bulletObject.transform.forward, ForceMode.Impulse);
        EffectManager.instance.PlayBulletSpark(shootPoint.position);
    }
}
