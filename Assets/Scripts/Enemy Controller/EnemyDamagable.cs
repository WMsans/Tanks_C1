using UnityEngine;

public class EnemyDamagable : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;

    public float Health { get => health; set => health = value; }
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
        EffectManager.instance.PlayExplosion(transform.position);
        LevelManager.Instance?.OnEnemyDie(this.gameObject);
        Destroy(gameObject);
        return;
    }
}
