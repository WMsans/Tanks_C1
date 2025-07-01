using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : MonoBehaviour, IPickable
{
    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private float healAmount;
    public void OnPickup(GameObject newOwner)
    {
        var harmable = newOwner.transform.root.GetComponentInChildren<IDamageable>();
        if(harmable == null) return;
        harmable.Health += healAmount;
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
