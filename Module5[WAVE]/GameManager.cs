using System;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Gameplay,
    GameOver,
    GameWin
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Cấu hình Mạng Căn Cứ")]
    [SerializeField] private int maxBaseHealth = 10;
    private int currentBaseHealth;

    public GameState CurrentState { get; private set; } = GameState.Gameplay;

    public static event Action<int, int> OnBaseHealthChanged;
    public static event Action OnGameOver;
    public static event Action OnGameWin;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentBaseHealth = maxBaseHealth;
        OnBaseHealthChanged?.Invoke(currentBaseHealth, maxBaseHealth);
    }

    public void DecreaseBaseHealth(int damage = 1)
    {
        if (CurrentState != GameState.Gameplay) return;

        currentBaseHealth = Mathf.Clamp(currentBaseHealth - damage, 0, maxBaseHealth);

        OnBaseHealthChanged?.Invoke(currentBaseHealth, maxBaseHealth);

        Debug.Log($"[GameManager] Căn cứ bị tấn công! Mạng còn lại: {currentBaseHealth}/{maxBaseHealth}");

        if (currentBaseHealth <= 0)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        CurrentState = GameState.GameOver;
        //Debug.LogError("=== GAME OVER! Căn cứ đã bị phá hủy ===");

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetState(GameState.GameOver);
        }

        OnGameOver?.Invoke();
    }

    public void TriggerGameWin()
    {
        if (CurrentState != GameState.Gameplay) return;

        CurrentState = GameState.GameWin;
        Debug.Log("=== VICTORY! Bạn đã dọn sạch toàn bộ quái ===");

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetState(GameState.GameWin);
        }

        OnGameWin?.Invoke();
    }
}