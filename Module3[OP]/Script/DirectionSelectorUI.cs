using System.Collections.Generic;
using UnityEngine;

public class DirectionSelectorUI : MonoBehaviour
{
    public static DirectionSelectorUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject selectorPanel;
    [SerializeField] private GameObject confirmDeployButton;

    private OperatorController pendingOperator;
    private List<GridNode> currentHighlightedNodes = new List<GridNode>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (selectorPanel != null)
            selectorPanel.SetActive(false);

        if (confirmDeployButton != null)
            confirmDeployButton.SetActive(false);
    }

    public void ShowSelector(OperatorController op)
    {
        if (op == null)
            return;

        pendingOperator = op;

        transform.position =
            op.transform.position + new Vector3(0, 0.5f, 0);

        if (selectorPanel != null)
            selectorPanel.SetActive(true);

        if (confirmDeployButton != null)
            confirmDeployButton.SetActive(true);

        // Mặc định preview hướng Up
        PreviewDirection(Direction.Up);
    }

    public void PreviewDirection(Direction dir)
    {
        if (pendingOperator == null)
            return;

        // Xóa range preview cũ
        ClearCurrentHighlight();

        // Chỉ đổi hướng, CHƯA hoàn tất Deploy
        pendingOperator.SetDirection(dir);

        Vector2Int originCoord = new Vector2Int(
            pendingOperator.CurrentNode.xIndex,
            pendingOperator.CurrentNode.zIndex
        );

        // Tính range theo hướng đang preview
        currentHighlightedNodes =
            GridManager.Instance.GetNodesInRange(
                originCoord,
                pendingOperator.Data.relativeAttackRange,
                dir
            );

        // Hiển thị range
        GridManager.Instance.HighlightRangeNodes(
            currentHighlightedNodes,
            true
        );
    }

    private void ClearCurrentHighlight()
    {
        if (currentHighlightedNodes == null ||
            currentHighlightedNodes.Count == 0)
            return;

        GridManager.Instance.HighlightRangeNodes(
            currentHighlightedNodes,
            false
        );

        currentHighlightedNodes.Clear();
    }
    public void PreviewUp()
    {
        PreviewDirection(Direction.Up);
    }

    public void PreviewDown()
    {
        PreviewDirection(Direction.Down);
    }

    public void PreviewLeft()
    {
        PreviewDirection(Direction.Left);
    }

    public void PreviewRight()
    {
        PreviewDirection(Direction.Right);
    }

    // Gọi bởi ConfirmDeployButton
    public void ConfirmDeploy()
    {
        if (pendingOperator == null)
            return;

        Debug.Log(
            $"Xác nhận Deploy: " +
            $"{pendingOperator.Data.operatorName} " +
            $"| Direction: {pendingOperator.FacingDirection}"
        );

        // Xóa preview range
        ClearCurrentHighlight();

        // Tắt UI
        if (selectorPanel != null)
            selectorPanel.SetActive(false);

        if (confirmDeployButton != null)
            confirmDeployButton.SetActive(false);

        // Báo PlacementManager rằng Deploy đã hoàn tất
        if (PlacementManager.Instance != null)
        {
            PlacementManager.Instance.FinishDirectionSelection();
        }

        pendingOperator = null;
    }
}