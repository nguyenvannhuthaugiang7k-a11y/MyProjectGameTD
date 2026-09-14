using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OperatorController))]
public class OperatorBlocker : MonoBehaviour
{
    public List<Enemy> blockedEnemies = new List<Enemy>();
    
    private OperatorController controller;
    private int maxBlockCount = 0;

    private void Awake()
    {
        controller = GetComponent<OperatorController>();
    }

    private void Start()
    {
        if (controller.Data != null)
        {
            maxBlockCount = controller.Data.blockCount;
        }
    }

    // Kiểm tra xem Operator còn sức cản quái không
    public bool CanBlockMore()
    {
        return blockedEnemies.Count < maxBlockCount;
    }

    private void Update()
    {
        if (controller.CurrentNode == null || !CanBlockMore()) return;

        // Quét quái vật đang đi vào ô hiện tại mà Operator đang đứng
        Vector3 tileCenter = controller.CurrentNode.worldPosition;
        Collider[] colliders = Physics.OverlapBox(tileCenter, new Vector3(0.45f, 1f, 0.45f), Quaternion.identity);

        foreach (var col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null &&
                enemy.CurrentHealth > 0f &&
                !enemy.isBlocked &&
                CanBlockMore())
            {
                BlockEnemy(enemy);
            }
        }
    }

    private void BlockEnemy(Enemy enemy)
    {
        enemy.isBlocked = true;
        enemy.blockedByOperator = controller;
        blockedEnemies.Add(enemy);
        Debug.Log($"{gameObject.name} đã Block thành công quái {enemy.name}! ({blockedEnemies.Count}/{maxBlockCount})");
    }

    public void UnblockEnemy(Enemy enemy)
    {
        if (blockedEnemies.Contains(enemy))
        {
            enemy.isBlocked = false;
            enemy.blockedByOperator = null;
            blockedEnemies.Remove(enemy);
            Debug.Log($"{gameObject.name} đã bỏ Block quái {enemy.name}.");
        }
    }
}