using UnityEngine;

[RequireComponent(typeof(OperatorController))]
public class OperatorHealth : MonoBehaviour
{
    [Header("Chỉ số HP")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Thanh máu")]
    [SerializeField] private HealthBar healthBar;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;

    private bool isDead = false;
    private OperatorController controller;
    private Animator animator;

    private void Awake()
    {
        controller = GetComponent<OperatorController>();
        animator = GetComponent<Animator>();

    }

    private void Start()
    {   
         if (controller.Data != null)
        {
            maxHealth = controller.Data.maxHealth;
        }

        CurrentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float amount)
    {
        if (isDead || CurrentHealth <= 0f)
            return;

        if (amount <= 0f)
            return;

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0f);

        //animator.SetTrigger("Hit");

        UpdateHealthBar();

        Debug.Log(
            $"[OperatorHealth] {gameObject.name} nhận {amount} damage. " +
            $"HP: {CurrentHealth}/{maxHealth}"
        );

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead || CurrentHealth <= 0f)
            return;

        if (amount <= 0f)
            return;

        CurrentHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth, maxHealth);

        UpdateHealthBar();

        Debug.Log(
            $"[OperatorHealth] {gameObject.name} được hồi {amount} HP. " +
            $"HP: {CurrentHealth}/{maxHealth}"
        );
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(
                CurrentHealth,
                maxHealth
            );
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(
            $"[OperatorHealth] {gameObject.name} đã hết HP!"
        );

        if (animator != null)
        {
            //controller.Retreat();
            animator.SetTrigger("Death");
        }
    }

    public void OnDeathAnimationFinished()
    {
        if (controller != null)
        {
            controller.Retreat();
        }
    }

}