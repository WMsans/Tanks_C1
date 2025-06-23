using System;
using UnityEngine;

public class TankAttack : MonoBehaviour
{
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float coolDown;
    private float _lastShootTime;
    private void OnAttack()
    {
        _lastShootTime = Time.time;
        Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
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
