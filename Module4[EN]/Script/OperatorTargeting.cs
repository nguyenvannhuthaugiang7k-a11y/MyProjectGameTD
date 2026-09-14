using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OperatorTargeting : MonoBehaviour
{
    public OperatorData operatorData;
    public Direction currentDirection = Direction.Up;
    public LayerMask enemyLayer;

    private OperatorController controller;
    private OperatorBlocker blocker;

    private void Awake()
    {
        controller = GetComponent<OperatorController>();
        blocker = GetComponent<OperatorBlocker>();
    }

    public Enemy GetPriorityTarget()
    {
        // 1. QUY TẮC ƯU TIÊN 1: Nếu có quái đang bị chính Operator này BLOCK -> Chém quái đó trước!
        if (blocker != null && blocker.blockedEnemies.Count > 0)
        {
            // Lọc quái còn sống trong danh sách Blocked
            Enemy targetInBlock = blocker.blockedEnemies.FirstOrDefault(e => e != null && e.CurrentHealth > 0);
            if (targetInBlock != null) return targetInBlock;
        }

        // 2. QUY TẮC ƯU TIÊN 2: Quét các quái trong tầm đánh và chọn con gần Căn cứ nhất
        List<Enemy> enemiesInRange = GetEnemiesInAttackRange();
        if (enemiesInRange.Count == 0) return null;

        return enemiesInRange.OrderBy(e => e.RemainingDistanceToBase).FirstOrDefault();
    }

    public List<Enemy> GetEnemiesInAttackRange()
    {
        List<Enemy> result = new List<Enemy>();

        if (controller == null || controller.CurrentNode == null) return result;

        Vector2Int originCoord = new Vector2Int(controller.CurrentNode.xIndex, controller.CurrentNode.zIndex);
        List<GridNode> rangeNodes = GridManager.Instance.GetNodesInRange(originCoord, operatorData.relativeAttackRange, currentDirection);

        foreach (GridNode node in rangeNodes)
        {
            Vector3 tileCenter = node.worldPosition;
            Collider[] colliders = Physics.OverlapBox(tileCenter, new Vector3(0.45f, 1f, 0.45f), Quaternion.identity, enemyLayer);

            foreach (var col in colliders)
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null && enemy.CurrentHealth > 0 && !result.Contains(enemy))
                {
                    result.Add(enemy);
                }
            }
        }

        return result;
    }
}