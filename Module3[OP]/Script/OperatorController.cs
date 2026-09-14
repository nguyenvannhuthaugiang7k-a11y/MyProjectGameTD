using UnityEngine;
using System.Collections.Generic;

// 4 Hướng nhìn đặc trưng trong Arknights
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class OperatorController : MonoBehaviour
{
    public OperatorData Data { get; private set; }
    public Direction FacingDirection { get; private set; }
    public GridNode CurrentNode { get; private set; }
    public OperatorCardUI SourceCard { get; private set; }

    private OperatorTargeting targetingSystem;

    private void Awake()
    {
        // Lấy tham chiếu tới component OperatorTargeting gắn trên cùng Prefab
        targetingSystem = GetComponent<OperatorTargeting>();
    }

    // Khởi tạo nhân vật với dữ liệu và ô đang đứng
    public void Setup(OperatorData data, GridNode node)
    {
        this.Data = data;
        this.CurrentNode = node;

        if (node != null)
        {
            node.isOccupied = true; // Đánh dấu ô này đã có nhân vật đứng
        }
    }

    public void SetSourceCard(OperatorCardUI sourceCard)
    {
        SourceCard = sourceCard;
    }

    // Đổi hướng nhìn cho nhân vật
    public void SetDirection(Direction dir)
    {
        FacingDirection = dir;

        // 1. Xoay mô hình 3D của nhân vật
        switch (dir)
        {
            case Direction.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Direction.Down:
                transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
            case Direction.Left:
                transform.rotation = Quaternion.Euler(0, 270, 0);
                break;
            case Direction.Right:
                transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
        }

        // 2. Cập nhật hướng nhìn sang hệ thống Target để tính toán lại ô tầm đánh
        if (targetingSystem != null)
        {
            targetingSystem.currentDirection = dir;
        }
    }

    public void Retreat()
    {
        // Gỡ Block tất cả Enemy đang bị Operator này chặn
        OperatorBlocker blocker = GetComponent<OperatorBlocker>();

        if (blocker != null)
        {
            // Tạo bản sao để tránh thay đổi List trong lúc foreach
            List<Enemy> enemiesToUnblock =
                new List<Enemy>(blocker.blockedEnemies);

            foreach (Enemy enemy in enemiesToUnblock)
            {
                if (enemy != null)
                {
                    blocker.UnblockEnemy(enemy);
                }
            }
        }

        // Giải phóng ô
        if (CurrentNode != null)
        {
            CurrentNode.isOccupied = false;
        }

        if (SourceCard != null)
        {
            SourceCard.StartCooldown();
        }

        // Xóa Operator
        Destroy(gameObject);
    }
}