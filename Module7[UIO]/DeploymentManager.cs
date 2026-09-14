using UnityEngine;
using System;

public class DeploymentManager : MonoBehaviour
{
    public static DeploymentManager Instance { get; private set; }

    [Header("Cấu hình DP (Deployment Points)")]
    [SerializeField] private int currentDP = 20;
    [SerializeField] private int maxDP = 99;
    [SerializeField] private float dpRecoveryInterval = 1f;
    
    private float dpTimer = 0f;

    [Header("Trạng thái Triển khai")]
    [SerializeField] private LayerMask tileLayerMask;
    private OperatorCardUI selectedCard;

    public int CurrentDP => currentDP;
    public static event Action<int> OnDPChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        OnDPChanged?.Invoke(currentDP);
    }

    private void Update()
    {
        // Chỉ hồi DP khi game đang chạy ở trạng thái Gameplay
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Gameplay) return;

        HandleDPRecovery();

        if (selectedCard != null)
        {
            HandlePlacementInput();
        }
    }

    private void HandleDPRecovery()
    {
        if (currentDP >= maxDP) return;

        dpTimer += Time.deltaTime;
        if (dpTimer >= dpRecoveryInterval)
        {
            dpTimer = 0f;
            AddDP(1);
        }
    }

    public void AddDP(int amount)
    {
        currentDP = Mathf.Clamp(currentDP + amount, 0, maxDP);
        OnDPChanged?.Invoke(currentDP);
    }

    public bool ConsumeDP(int amount)
    {
        if (currentDP >= amount)
        {
            currentDP -= amount;
            OnDPChanged?.Invoke(currentDP);
            return true;
        }
        return false;
    }

    public void SelectOperatorToDeploy(OperatorCardUI card)
    {
        selectedCard = card;
    }

    private void HandlePlacementInput()
    {
        // Tương thích cả Top-Down 2D và Top-Down 3D 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayerMask))
        {
            PlacementTile tile = hit.collider.GetComponent<PlacementTile>();

            if (Input.GetMouseButtonDown(0) && tile != null)
            {
                TryDeployOperator(tile);
            }
        }

        // Hủy chọn khi nhấp chuột phải
        if (Input.GetMouseButtonDown(1))
        {
            CancelDeployment();
        }
    }

    private void TryDeployOperator(PlacementTile tile)
    {
        if (selectedCard == null || selectedCard.data == null) return;
        OperatorData data = selectedCard.data;

        // 1. Kiểm tra DP
        if (currentDP < data.dpCost)
        {
            CancelDeployment();
            return;
        }

        // 2. Kiểm tra ô trống
        if (tile.isOccupied) return;

        // 3. Kiểm tra loại ô đất (Melee vs Ranged) Top-Down
        if (tile.tileType != data.allowedTileType) return;

        // --- ĐẶT THÀNH CÔNG ---
        if (ConsumeDP(data.dpCost))
        {
            Vector3 spawnPosition = tile.GetPlacementPosition();
            GameObject opObj = Instantiate(data.operatorPrefab, spawnPosition, Quaternion.identity);

            tile.isOccupied = true;
            tile.currentOperator = opObj;

            selectedCard.StartCooldown();
            CancelDeployment();
        }
    }

    public void CancelDeployment()
    {
        selectedCard = null;
    }
}