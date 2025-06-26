using UnityEngine;

public interface ITankAttack
{
    public float LastAttackTime { get; }
    public void OnAttack();
}
