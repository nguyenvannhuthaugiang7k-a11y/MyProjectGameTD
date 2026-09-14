using UnityEngine;

public class OperatorCombat : MonoBehaviour
{
    [Header("Chỉ số Tấn công")]
    public float attackDamage = 35f;
    public float attackInterval = 1.0f; // Tốc độ đánh
    private float attackTimer = 0f;

    [Header("Tham chiếu")]
    public Transform firePoint; // Vị trí đầu súng/bàn tay
    public GameObject projectilePrefab; // (Nếu là nhân vật bắn xa)

    private OperatorTargeting targeting;

    private void Awake()
    {
        targeting = GetComponent<OperatorTargeting>();
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        Enemy target = targeting.GetPriorityTarget();

        if (target != null && attackTimer >= attackInterval)
        {
            PerformAttack(target);
            attackTimer = 0f;
        }
    }

    private void PerformAttack(Enemy target)
    {
        // Chú ý: Nhân vật GIỮ NGUYÊN HƯỚNG NHÌN, KHÔNG gọi LookAt(target)

        if (projectilePrefab != null && firePoint != null)
        {
            // Nếu có đạn (Xạ thủ/Pháp sư)
            GameObject projObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile proj = projObj.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.Initialize(target, attackDamage);
            }
        }
        else
        {
            // Đánh cận chiến (Chém trực tiếp)
            target.TakeDamage(attackDamage);
            Debug.Log($"{gameObject.name} vừa chém {target.name} gây {attackDamage} sát thương!");
        }
    }
}