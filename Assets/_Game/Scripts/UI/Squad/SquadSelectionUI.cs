using UnityEngine;
using TMPro;

public class SquadSelectionUI : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private OperatorDatabase operatorDatabase;

    [Header("Card")]
    [SerializeField] private SquadOperatorCardUI cardPrefab;

    [Header("Card Parent")]
    [SerializeField] private Transform cardParent;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI selectedCountText;

    private void Start()
    {
        if (SquadManager.Instance == null)
        {
            Debug.LogError(
                "[SquadSelectionUI] Không tìm thấy SquadManager!"
            );

            return;
        }

        SquadManager.Instance.OnSquadChanged += RefreshUI;

        RefreshUI();

        GameplayPhaseManager.OnGameplayPhaseChanged +=
            HandlePhaseChanged;

        SpawnOperatorCards();
    }

    private void SpawnOperatorCards()
    {
        if (operatorDatabase == null)
        {
            Debug.LogError(
                "[SquadSelectionUI] Chưa gán OperatorDatabase!"
            );

            return;
        }

        if (cardPrefab == null)
        {
            Debug.LogError(
                "[SquadSelectionUI] Chưa gán Card Prefab!"
            );

            return;
        }

        if (cardParent == null)
        {
            Debug.LogError(
                "[SquadSelectionUI] Chưa gán Card Parent!"
            );

            return;
        }

        foreach (OperatorData data in operatorDatabase.Operators)
        {
            if (data == null)
                continue;

            SquadOperatorCardUI card =
                Instantiate(cardPrefab, cardParent);

            card.Setup(data);
        }

        Debug.Log(
            $"[SquadSelectionUI] Đã tạo " +
            $"{operatorDatabase.Operators.Count} Selection Card."
        );
    }

    private void RefreshUI()
    {
        if (SquadManager.Instance == null)
            return;

        int currentCount =
            SquadManager.Instance.GetSelectedCount();

        int maxCount =
            SquadManager.Instance.GetMaxSquadSize();

        if (selectedCountText != null)
        {
            selectedCountText.text =
                $"Squad: {currentCount} / {maxCount}";
        }
    }

    public void StartBattle()
    {
        if (SquadManager.Instance == null)
        {
            Debug.LogError(
                "[SquadSelectionUI] Không tìm thấy SquadManager!"
            );

            return;
        }

        if (SquadManager.Instance.GetSelectedCount() == 0)
        {
            Debug.Log(
                "[SquadSelectionUI] Chưa chọn Operator nào!"
            );

            return;
        }

        if (GameplayPhaseManager.Instance == null)
        {
            Debug.LogError(
                "[SquadSelectionUI] Không tìm thấy GameplayPhaseManager!"
            );

            return;
        }

        GameplayPhaseManager.Instance.StartBattle();
    }

    private void HandlePhaseChanged(GameplayPhase newPhase)
    {
        if (newPhase == GameplayPhase.Preparation)
        {
            gameObject.SetActive(true);
            RefreshUI();
        }

        if (newPhase == GameplayPhase.Battle)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (SquadManager.Instance != null)
        {
            SquadManager.Instance.OnSquadChanged -= RefreshUI;
        }

        GameplayPhaseManager.OnGameplayPhaseChanged -=
            HandlePhaseChanged;
    }
}