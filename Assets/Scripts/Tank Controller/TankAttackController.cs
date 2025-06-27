using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAttackController : MonoBehaviour
{
    [SerializeField] private ScriptableObject initialTankAttack;
    public ITankAttack TankAttack { get; private set; }

    private void Start()
    {
        if (initialTankAttack is ITankAttack tankAttack)
        {
            TankAttack = tankAttack;
        }
        else
        {
            Debug.LogError("Provided initial tank attack is not ITankAttack");
            return;
        }
    }

    public void SetTankAttack(ITankAttack newTankAttack)
    {
        TankAttack = newTankAttack;
    }
}
