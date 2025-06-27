using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackItem : MonoBehaviour, IPickable
{
    [SerializeField] private ScriptableObject lasorTankAttack;
    [SerializeField] private LayerMask characterLayer;
    public void OnPickup(GameObject newOwner)
    {
        if (lasorTankAttack is not ITankAttack tankAttack) return;
        var attackController = newOwner.GetComponentInChildren<TankAttackController>();
        attackController.SetTankAttack(tankAttack);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((characterLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            OnPickup(other.transform.root.gameObject);
        }
    }
}
