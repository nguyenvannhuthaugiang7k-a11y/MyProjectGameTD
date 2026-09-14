using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Chỉ số Cơ bản")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Trạng thái Block")]
    public bool isBlocked = false;
    public OperatorController blockedByOperator = null;

    [Header("Đường đi")]
    private EnemyPath enemyPath;
    private int currentWaypointIndex = 0;

    [Header("UI & Hiệu ứng")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject hitEffectPrefab;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;

    public float RemainingDistanceToBase { get; private set; }

    public event Action<Enemy> OnDespawned;

    private bool isDespawned = false;

    private void Start()
    {
        CurrentHealth = maxHealth;

        UpdateRemainingDistance();

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(CurrentHealth, maxHealth);
        }
    }

    private void Update()
    {
        if (isDespawned || isBlocked || CurrentHealth <= 0f)
        {
            return;
        }

        MoveAlongPath();
        UpdateRemainingDistance();
    }

    public void Initialize(EnemyPath path)
    {
        if (path == null)
        {
            Debug.LogError(
                $"[{nameof(Enemy)}] EnemyPath không được gán.",
                this
            );

            return;
        }

        enemyPath = path;
        currentWaypointIndex = 0;
        isDespawned = false;
        CurrentHealth = maxHealth;

        UpdateRemainingDistance();

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(CurrentHealth, maxHealth);
        }
    }

    private void MoveAlongPath()
    {
        if (enemyPath == null ||
            enemyPath.waypoints == null ||
            currentWaypointIndex >= enemyPath.waypoints.Count)
        {
            return;
        }

        Transform targetNode = enemyPath.waypoints[currentWaypointIndex];

        if (targetNode == null)
        {
            currentWaypointIndex++;
            return;
        }

        Vector3 direction =
            (targetNode.position - transform.position).normalized;

        transform.position +=
            direction * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetNode.position) < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= enemyPath.waypoints.Count)
            {
                ReachBlueBox();
            }
        }
    }

    private void ReachBlueBox()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DecreaseBaseHealth(1);
        }

        Despawn(true);
    }

    private void UpdateRemainingDistance()
    {
        if (enemyPath == null)
        {
            RemainingDistanceToBase = 0f;
            return;
        }

        RemainingDistanceToBase =
            enemyPath.GetRemainingDistance(
                currentWaypointIndex,
                transform.position
            );
    }

    public void TakeDamage(float amount, Vector3 hitPosition = default)
    {
        if (isDespawned || CurrentHealth <= 0f)
        {
            return;
        }

        if (amount <= 0f)
        {
            return;
        }

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(0f, CurrentHealth);

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(
                CurrentHealth,
                maxHealth
            );
        }

        SpawnHitEffect(hitPosition);

        if (CurrentHealth <= 0f)
        {
            Despawn();
        }
    }

    private void SpawnHitEffect(Vector3 hitPosition)
    {
        if (hitEffectPrefab == null)
        {
            return;
        }

        Vector3 spawnPosition =
            hitPosition != default
                ? hitPosition
                : transform.position + Vector3.up * 0.5f;

        GameObject vfx = Instantiate(
            hitEffectPrefab,
            spawnPosition,
            Quaternion.identity
        );

        Destroy(vfx, 1f);
    }

    private void Despawn(bool reachedGoal = false)
    {
        if (isDespawned)
        {
            return;
        }

        isDespawned = true;

        if (isBlocked && blockedByOperator != null)
        {
            OperatorBlocker blocker =
                blockedByOperator.GetComponent<OperatorBlocker>();

            if (blocker != null)
            {
                blocker.UnblockEnemy(this);
            }
        }

        OnDespawned?.Invoke(this);

        Destroy(gameObject);
    }
}