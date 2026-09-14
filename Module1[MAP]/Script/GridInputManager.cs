using UnityEngine;

public class GridInputManager : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private LayerMask tileLayerMask; // Layer để lọc chỉ Raycast trúng Ô bản đồ

    [Header("Operator Selection UI")]
    [SerializeField] private GameObject retreatButton;
    [SerializeField] private GameObject cancelRetreatButton;

    private Camera mainCamera;
    private TileVisual currentHoveredTile;
    private OperatorController selectedOperator;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleMouseHover();
        HandleMouseClick();
    }

    // Xử lý khi rê chuột qua ô
    private void HandleMouseHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, tileLayerMask))
        {
            TileVisual tile = hitInfo.collider.GetComponent<TileVisual>();

            if (tile != null)
            {
                if (currentHoveredTile != tile)
                {
                    // Bỏ highlight ô cũ
                    if (currentHoveredTile != null)
                    {
                        currentHoveredTile.SetHover(false);
                    }

                    // Highlight ô mới
                    currentHoveredTile = tile;
                    currentHoveredTile.SetHover(true);
                }
            }
        }
        else
        {
            // Nếu di chuột ra ngoài bản đồ, tắt highlight ô cũ
            if (currentHoveredTile != null)
            {
                currentHoveredTile.SetHover(false);
                currentHoveredTile = null;
            }
        }
    }
    

    // Xử lý khi nhấp chuột trái vào ô
    private void HandleMouseClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        // Ưu tiên kiểm tra Operator trước
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
        {
            OperatorController operatorController =
                hitInfo.collider.GetComponentInParent<OperatorController>();

            if (operatorController != null)
            {
                selectedOperator = operatorController;

                Debug.Log(
                    $"Đã chọn Operator: " +
                    $"{operatorController.Data.operatorName}"
                );

                if (PlacementManager.Instance != null)
                {
                    PlacementManager.Instance.StartRetreatSelection();
                }

                if (retreatButton != null)
                    retreatButton.SetActive(true);

                if (cancelRetreatButton != null)
                    cancelRetreatButton.SetActive(true);

                return;
            }
        }

        // Nếu không click vào Operator thì xử lý Tile như cũ
        if (currentHoveredTile != null)
        {
            GridNode node = currentHoveredTile.Node;

            if (PlacementManager.Instance != null)
                PlacementManager.Instance.TryPlaceOperator(node);
        }
    }

    public void RetreatSelectedOperator()
    {
        if (selectedOperator == null)
            return;

        OperatorData data = selectedOperator.Data;

        Debug.Log(
            $"Retreat Operator: " +
            $"{data.operatorName}"
        );

        // Hoàn 50% DP
        int refundAmount = Mathf.FloorToInt(data.dpCost * 0.5f);

        if (DPManager.Instance != null)
        {
            DPManager.Instance.RefundDP(refundAmount);
        }

        // Xóa Operator và giải phóng ô
        selectedOperator.Retreat();

        if (PlacementManager.Instance != null)
        {
            PlacementManager.Instance.FinishRetreatSelection();
        }

        selectedOperator = null;

        if (retreatButton != null)
            retreatButton.SetActive(false);

        if (cancelRetreatButton != null)
            cancelRetreatButton.SetActive(false);
    }

    public void CancelRetreatSelection()
    {
        if (selectedOperator == null)
            return;

        Debug.Log(
            $"Hủy chọn Retreat: " +
            $"{selectedOperator.Data.operatorName}"
        );

        selectedOperator = null;

        // Trả game về tốc độ bình thường
        Time.timeScale = 1f;

        // Ẩn các nút
        if (retreatButton != null)
            retreatButton.SetActive(false);

        if (cancelRetreatButton != null)
            cancelRetreatButton.SetActive(false);
    }
}