using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance { get; private set; }

    [Header("Testing Selected Operator")]
    [SerializeField] private OperatorData currentSelectedOperator;

    [Header("Placement State")]
    [HideInInspector] public bool isSelectingDirection = false;

    [Header("Deploy Slow Motion")]
    [SerializeField] private float deployTimeScale = 0.1f;

    // Operator đang chờ người chơi xác nhận hướng
    private OperatorController pendingOperator;
    private OperatorCardUI pendingSourceCard;

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

    private void Update()
    {
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (isSelectingDirection)
        {
            return;
        }
    }

    public void SelectOperatorToPlace(
    OperatorData data,
    OperatorCardUI sourceCard)
    {
        if (data == null)
        {
            Debug.LogWarning("Không thể chọn Operator: Data null!");
            return;
        }

        currentSelectedOperator = data;
        pendingSourceCard = sourceCard;

        Debug.Log(
            $"Đã chọn Operator: {data.operatorName} " +
            $"| Class: {data.operatorClass} " +
            $"| Terrain: {data.allowedTileType}"
        );
    }

    public bool TryPlaceOperator(GridNode node)
    {
        if (isSelectingDirection)
            return false;

        if (currentSelectedOperator == null)
        {
            Debug.Log("Chưa chọn nhân vật nào để đặt!");
            return false;
        }

        if (node == null)
        {
            Debug.LogWarning("GridNode không hợp lệ!");
            return false;
        }

        if (node.isOccupied)
        {
            Debug.Log("Ô này đã có nhân vật!");
            return false;
        }

        if (!CanOperatorBePlacedOnTile(
            currentSelectedOperator,
            node))
        {
            Debug.Log(
                $"Không thể đặt {currentSelectedOperator.operatorName} " +
                $"({currentSelectedOperator.operatorClass}) " +
                $"trên ô {node.tileType}!"
            );

            return false;
        }

        if (DPManager.Instance == null)
        {
            Debug.LogError("Không tìm thấy DPManager!");
            return false;
        }

        if (!DPManager.Instance.HasEnoughDP(
            currentSelectedOperator.dpCost))
        {
            Debug.Log("Không đủ DP!");
            return false;
        }

        // Lưu data trước khi clear
        OperatorData selectedData = currentSelectedOperator;

        // Spawn Operator
        GameObject opObj = Instantiate(
            selectedData.operatorPrefab,
            node.worldPosition,
            Quaternion.identity
        );

        if (opObj == null)
        {
            Debug.LogError("Không thể tạo Operator!");
            return false;
        }

        OperatorController opController =
            opObj.GetComponent<OperatorController>();

        if (opController == null)
        {
            Debug.LogWarning(
                $"Prefab {opObj.name} không có OperatorController!"
            );

            Destroy(opObj);
            return false;
        }

        // Setup Operator
        opController.Setup(
            selectedData,
            node
        );

        opController.SetSourceCard(pendingSourceCard);

        // Đánh dấu ô đã có Operator
        node.isOccupied = true;

        // Operator đang chờ xác nhận hướng
        pendingOperator = opController;
        isSelectingDirection = true;

        Time.timeScale = deployTimeScale;

        // Hiển thị Direction Selector
        if (DirectionSelectorUI.Instance != null)
        {   
            isSelectingDirection = true;

            DirectionSelectorUI.Instance.ShowSelector(
                opController
            );
        }

        // Clear Operator Card đang chọn
        currentSelectedOperator = null;

        Debug.Log(
            $"Đã đặt {selectedData.operatorName} vào ô " +
            $"[{node.xIndex}, {node.zIndex}]. " +
            $"Đang chờ xác nhận hướng."
        );

        return true;
    }

    private bool CanOperatorBePlacedOnTile(
        OperatorData operatorData,
        GridNode node)
    {
        if (operatorData == null || node == null)
            return false;

        if (node.tileType == TileType.LowGround)
        {
            if (operatorData.operatorClass == OperatorClass.Melee ||
                operatorData.operatorClass == OperatorClass.Defender)
            {
                return true;
            }

            return false;
        }

        if (node.tileType == TileType.HighGround)
        {
            switch (operatorData.operatorClass)
            {
                case OperatorClass.Ranger:
                case OperatorClass.Caster:
                case OperatorClass.Medic:
                case OperatorClass.Supporter:
                    return true;

                default:
                    return false;
            }
        }

        if (node.tileType == TileType.Blocked)
            return false;

        return false;
    }

    public void FinishDirectionSelection()
    {
        if (pendingOperator == null)
        {   
            Time.timeScale = 1f;
            isSelectingDirection = false;
            return;
        }

        // Chỉ ở bước này mới trừ DP
        if (DPManager.Instance != null &&
            pendingOperator.Data != null)
        {
            DPManager.Instance.ConsumeDP(
                pendingOperator.Data.dpCost
            );
        }

        Debug.Log(
            $"Deploy hoàn tất: " +
            $"{pendingOperator.Data.operatorName}"
        );

        pendingOperator = null;
        isSelectingDirection = false;
        Time.timeScale = 1f;
    }

    public void StartRetreatSelection()
    {
        Time.timeScale = deployTimeScale;

        Debug.Log("Bắt đầu chọn Retreat - Slow Motion");
    }

    public void FinishRetreatSelection()
    {
        Time.timeScale = 1f;

        Debug.Log("Kết thúc Retreat - Game trở lại bình thường");
    }
}