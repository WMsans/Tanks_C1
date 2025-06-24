using System;
using UnityEngine;

public class TankAttack : MonoBehaviour
{
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float coolDown;
    [SerializeField] private float firePower;
    private float _lastShootTime;
    private void OnAttack()
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

    private void Update()
    {
        HandleAttack();
    }

    private void HandleAttack()
    {
        if (InputSystemManager.Instance.CurrentInputInfo.AttackDown)
        {
            if (CanShoot())
            {
                OnAttack();
            }
        }
    }

    private bool CanShoot()
    {
        return Time.time - _lastShootTime > coolDown;
    }
}
