using UnityEngine;

public interface ITankAttack
{
    public float CoolDown { get; }
    public void OnAttack(TankAttackController controller);
    public void OnEquip(TankAttackController controller);
    public void OnUnequip(TankAttackController controller);
}