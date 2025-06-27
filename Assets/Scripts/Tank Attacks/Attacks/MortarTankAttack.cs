using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/New Mortar Attack")]
public class MortarTankAttack : ScriptableObject, ITankAttack
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireHeight;
    [SerializeField] private float coolDown;
    [Tooltip("Set this to the layer your ground is on for accurate targeting.")]
    [SerializeField] private LayerMask groundMask;

    public float CoolDown => coolDown;
    public void OnAttack(Transform shootPoint)
    {

        var bulletObject = PoolManager.instance.GetPooledObject(bulletPrefab);
        bulletObject.transform.position = shootPoint.transform.position;
        bulletObject.transform.rotation = shootPoint.transform.rotation;

        var rb = bulletObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Bullet Prefab is missing a Rigidbody component!");
            bulletObject.SetActive(true); 
            return;
        }

        var mouseScreenPos = InputSystemManager.Instance.CurrentInputInfo.MousePosition;
        var ray = Camera.main.ScreenPointToRay(mouseScreenPos);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            var targetPosition = hitInfo.point;
            CalculateAndApplyVelocity(rb, shootPoint.position, targetPosition);

            bulletObject.SetActive(true);
            EffectManager.instance.PlayBulletSpark(shootPoint.position);
        }
        else
        {

            Debug.LogWarning("Mortar target not found on ground layer.");
            bulletObject.SetActive(false);
        }
    }

    private void CalculateAndApplyVelocity(Rigidbody rb, Vector3 startPosition, Vector3 targetPosition)
    {

        var gravity = Physics.gravity.y;

        var initialYVelocity = Mathf.Sqrt(fireHeight * -2f * gravity);

        var displacement = targetPosition - startPosition;
        var time = (initialYVelocity + Mathf.Sqrt(Mathf.Pow(initialYVelocity, 2) + 2f * gravity * displacement.y)) / -gravity;

        var horizontalVelocity = new Vector3(displacement.x / time, 0, displacement.z / time);

        rb.linearVelocity = horizontalVelocity + Vector3.up * initialYVelocity;
    }
}