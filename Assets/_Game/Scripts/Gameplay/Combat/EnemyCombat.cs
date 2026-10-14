using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyCombat : MonoBehaviour
{
    [Header("Chỉ số tấn công")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackInterval = 1.5f;

    private Enemy enemy;
    private float attackTimer = 0f;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (enemy == null)
            return;

        // Enemy chưa bị Block
        if (!enemy.isBlocked)
        {
            attackTimer = 0f;
            return;
        }

        // Không có Operator đang Block
        if (enemy.blockedByOperator == null)
        {
            attackTimer = 0f;
            return;
        }

        OperatorHealth operatorHealth =
            enemy.blockedByOperator.GetComponent<OperatorHealth>();

        if (operatorHealth == null)
            return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            Attack(operatorHealth);
            attackTimer = 0f;
        }
    }

    private void Attack(OperatorHealth target)
    {
        if (target == null)
            return;

        target.TakeDamage(attackDamage);

        Debug.Log(
            $"[EnemyCombat] {gameObject.name} tấn công " +
            $"{target.gameObject.name} gây {attackDamage} damage."
        );
    }
}