using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/New Normal Attack")]
public class TankAttack : ScriptableObject, ITankAttack
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float firePower;
    [SerializeField] private float coolDown;
    public float CoolDown => coolDown;
    public void OnAttack(TankAttackController controller)
    {
        // var bulletObject = Instantiate(bulletPrefab);
        var bulletObject = PoolManager.instance.GetPooledObject(bulletPrefab);
        bulletObject.transform.position = controller.FirePoint.transform.position;
        bulletObject.transform.rotation = controller.FirePoint.transform.rotation;
        bulletObject.SetActive(true);
        EffectManager.instance.PlayBulletSpark(controller.FirePoint.transform.position);
        if (firePower < 0.1f) return;
        var bulletRb = bulletObject.GetComponent<Rigidbody>();
        if (!bulletRb) return;
        bulletRb.AddForce(firePower * bulletObject.transform.forward, ForceMode.Impulse);
    }

    public void OnEquip(TankAttackController controller)
    {
    }

    public void OnUnequip(TankAttackController controller)
    {
    }
}