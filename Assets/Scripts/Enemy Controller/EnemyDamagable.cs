using UnityEngine;
using UnityEngine.Events;

public class EnemyDamagable : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;
    public float Health { get => health; set => health = value; }
    private bool dead = false;
    public UnityEvent onDeath { get; private set; } = new();
    public bool OnHit(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            OnDeath();
            return true;
        }

        return false;
    }
    private void OnDeath()
    {
        if(dead) return;
        EffectManager.instance.PlayExplosion(transform.position);
        onDeath.Invoke();
        dead = true;
        Destroy(gameObject);
        return;
    }
}
