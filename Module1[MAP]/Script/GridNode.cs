using UnityEngine;

// Định nghĩa các loại ô trong game phong cách Arknights
public enum TileType
{
    LowGround,
    HighGround,
    Blocked
}

public class GridNode
{
    public int xIndex;            // Tọa độ X trong mảng 2D
    public int zIndex;            // Tọa độ Z trong mảng 2D
    public Vector3 worldPosition; // Vị trí thực tế trong không gian 3D Unity
    public TileType tileType;     // Loại ô
    public bool isEnemyPath;
    public bool isOccupied;       // Trạng thái: đã có nhân vật đặt lên chưa?

    // Constructor để khởi tạo dữ liệu cho từng ô
    public GridNode(int x, int z, Vector3 worldPos, TileType type)
    {
        this.xIndex = x;
        this.zIndex = z;
        this.worldPosition = worldPos;
        this.tileType = type;
        this.isEnemyPath = false;
        this.isOccupied = false;
    }
}