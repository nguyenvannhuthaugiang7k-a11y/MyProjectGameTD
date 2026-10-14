using UnityEngine;

public class OperatorCombat : MonoBehaviour
{
    private float attackTimer = 0f;
    private float attackDamage;
    private float attackInterval;

    [Header("Tham chiếu")]
    public Transform firePoint; // Vị trí đầu súng/bàn tay
    public GameObject projectilePrefab; // (Nếu là nhân vật bắn xa)

    private OperatorTargeting targeting;
    private Animator animator;

    private void Awake()
    {
            targeting = GetComponent<OperatorTargeting>();
            animator = GetComponent<Animator>();
    }

    /*private void Start()
    {
        OperatorController controller = GetComponent<OperatorController>();

        if (controller != null && controller.Data != null)
        {
            attackDamage = controller.Data.attackDamage;
            attackInterval = controller.Data.attackInterval;
        }
    }*/
    private void Start()
    {
        OperatorController controller = GetComponent<OperatorController>();

        if (controller != null && controller.Data != null)
        {
            attackDamage = controller.Data.attackDamage;
            attackInterval = controller.Data.attackInterval;
        }

        if (firePoint == null)
        {
            Transform[] allChildren = GetComponentsInChildren<Transform>(true);

            foreach (Transform child in allChildren)
            {
                if (child.name == "FirePoint")
                {
                    firePoint = child;
                    break;
                }
            }
        }
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
        animator.SetTrigger("Attack");

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
            //Debug.Log($"{gameObject.name} vừa chém {target.name} gây {attackDamage} sát thương!");
        }
    }
}