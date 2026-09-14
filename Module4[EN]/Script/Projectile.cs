using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    private Enemy targetEnemy;
    private float damage;

    public void Initialize(Enemy target, float dmg)
    {
        targetEnemy = target;
        damage = dmg;
    }

    private void Update()
    {
        if (targetEnemy == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPos = targetEnemy.transform.position + Vector3.up * 0.5f;
        Vector3 dir = (targetPos - transform.position).normalized;

        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPos) < 0.2f)
        {
            // Truyền vị trí đạn nổ/chạm vào hàm TakeDamage
            targetEnemy.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
    }
}