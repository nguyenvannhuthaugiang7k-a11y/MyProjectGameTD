using UnityEngine;

public class DeployCardSpawner : MonoBehaviour
{
    [Header("Card Prefab")]
    [SerializeField] private OperatorCardUI cardPrefab;

    [Header("Card Parent")]
    [SerializeField] private Transform cardParent;

    private void Start()
    {
        if (GameplayPhaseManager.Instance == null)
        {
            Debug.LogError(
                "[DeployCardSpawner] Không tìm thấy GameplayPhaseManager!"
            );

            return;
        }

        GameplayPhaseManager.OnGameplayPhaseChanged +=
            HandlePhaseChanged;
    }

    private void HandlePhaseChanged(GameplayPhase newPhase)
    {
        if (newPhase == GameplayPhase.Battle)
        {
            SpawnDeployCards();
        }
    }

    private void SpawnDeployCards()
    {
        if (SquadManager.Instance == null)
        {
            Debug.LogError(
                "[DeployCardSpawner] Không tìm thấy SquadManager!"
            );

            return;
        }

        if (cardPrefab == null)
        {
            Debug.LogError(
                "[DeployCardSpawner] Chưa gán Card Prefab!"
            );

            return;
        }

        if (cardParent == null)
        {
            Debug.LogError(
                "[DeployCardSpawner] Chưa gán Card Parent!"
            );

            return;
        }

        foreach (OperatorData data in
                 SquadManager.Instance.SelectedOperators)
        {
            if (data == null)
                continue;

            OperatorCardUI card =
                Instantiate(cardPrefab, cardParent);

            card.Setup(data);

            Debug.Log(
                $"[DeployCardSpawner] Đã tạo Card: {data.operatorName}"
            );
        }
    }

    private void OnDestroy()
    {
        GameplayPhaseManager.OnGameplayPhaseChanged -=
            HandlePhaseChanged;
    }
}