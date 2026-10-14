using System;
using UnityEngine;

public enum GameplayPhase
{
    Preparation,
    Battle
}

public class GameplayPhaseManager : MonoBehaviour
{
    public static GameplayPhaseManager Instance { get; private set; }
    public static event Action<GameplayPhase> OnGameplayPhaseChanged;
    public GameplayPhase CurrentPhase { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        CurrentPhase = GameplayPhase.Preparation;
    }

    public void StartBattle()
    {
        if (CurrentPhase != GameplayPhase.Preparation)
            return;

        CurrentPhase = GameplayPhase.Battle;

        if (DPManager.Instance != null)
        {
            DPManager.Instance.StartDPRecovery();
        }

        Debug.Log(
            "Gameplay Phase: PREPARATION → BATTLE"
        );

        OnGameplayPhaseChanged?.Invoke(CurrentPhase);
    }

    public bool IsPreparation()
    {
        return CurrentPhase == GameplayPhase.Preparation;
    }

    public bool IsBattle()
    {
        return CurrentPhase == GameplayPhase.Battle;
    }

    private void Start()
    {
        Debug.Log(
            $"Gameplay Phase hiện tại: {CurrentPhase}"
        );
    }

}