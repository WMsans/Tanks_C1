using UnityEngine;

public class TankAttackController : MonoBehaviour
{
    [SerializeField] private ScriptableObject initialTankAttack;
    [SerializeField] private Transform initialFirePoint;
    public ITankAttack TankAttack { get; private set; }
    public Transform FirePoint { get; set; }
    public GameObject indicatorInstance;

    private void Awake()
    {
        if (initialFirePoint) FirePoint = initialFirePoint;
        if (initialTankAttack is ITankAttack tankAttack)
        {
            SetTankAttack(tankAttack);
        }
        else
        {
            Debug.LogError("Provided initial tank attack is not ITankAttack");
            return;
        }
    }

    public void SetTankAttack(ITankAttack newTankAttack)
    {
        TankAttack?.OnUnequip(this);
        TankAttack = newTankAttack;
        TankAttack.OnEquip(this);
    }
}