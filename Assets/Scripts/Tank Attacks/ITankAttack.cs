using UnityEngine;

public interface ITankAttack
{
    public float CoolDown { get; }
    public void OnAttack(Transform shootPoint);
}
