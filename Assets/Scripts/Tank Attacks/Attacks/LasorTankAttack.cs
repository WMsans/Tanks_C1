using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/New Lasor Attack")]
public class LasorTankAttack : ScriptableObject, ITankAttack
{
    [Header("Prefabs")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject indicatorPrefab;

    [Header("Stats")]
    [SerializeField] private float coolDown;
    [Tooltip("How long the laser beam lasts. Should match the 'lasorTime' in LasorBulletBehaviour.")]
    [SerializeField] private float laserDuration = 0.2f;

    public float CoolDown => coolDown;

    public void OnEquip(TankAttackController controller)
    {
        if (indicatorPrefab != null)
        {
            if(controller.indicatorInstance != null) Destroy(controller.indicatorInstance);
            controller.indicatorInstance = Instantiate(indicatorPrefab, controller.FirePoint.position, controller.FirePoint.rotation, controller.FirePoint);
        }
    }

    public void OnUnequip(TankAttackController controller)
    {
        if (controller.indicatorInstance != null)
        {
            Destroy(controller.indicatorInstance);
        }
    }

    public void OnAttack(TankAttackController controller)
    {
        controller.StartCoroutine(AttackSequence(controller));
    }

    private IEnumerator AttackSequence(TankAttackController controller)
    {
        if (controller.indicatorInstance != null)
        {
            controller.indicatorInstance.SetActive(false);
        }

        var laserObject = PoolManager.instance.GetPooledObject(laserPrefab);
        laserObject.transform.position = controller.FirePoint.position;
        laserObject.transform.rotation = controller.FirePoint.rotation;
        laserObject.SetActive(true);
        EffectManager.instance.PlayBulletSpark(controller.FirePoint.position);

        yield return new WaitForSeconds(laserDuration);

        if (controller.indicatorInstance != null)
        {
            controller.indicatorInstance.SetActive(true);
        }
    }
}