using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/New Normal Attack")]
public class TankAttack : ScriptableObject, ITankAttack
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float firePower;
    [SerializeField] private float coolDown;
    public float CoolDown => coolDown;
    public void OnAttack(Transform shootPoint)
    {
        // var bulletObject = Instantiate(bulletPrefab);
        var bulletObject = PoolManager.instance.GetPooledObject(bulletPrefab);
        bulletObject.transform.position = shootPoint.transform.position;
        bulletObject.transform.rotation = shootPoint.transform.rotation;
        bulletObject.SetActive(true);
        EffectManager.instance.PlayBulletSpark(shootPoint.position);
        if (firePower < 0.1f) return;
        var bulletRb = bulletObject.GetComponent<Rigidbody>();
        if (!bulletRb) return;
        bulletRb.AddForce(firePower * bulletObject.transform.forward, ForceMode.Impulse);
    }
}
