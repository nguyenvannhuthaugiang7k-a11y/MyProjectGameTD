using UnityEngine;

public class TileVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Color Settings")]
    [SerializeField] private Color meleeColor = new Color(0.8f, 0.8f, 0.8f, 0.3f);   // Xám nhạt - Ô đất liền
    [SerializeField] private Color rangedColor = new Color(0.2f, 0.6f, 1.0f, 0.3f);  // Xanh dương - Ô cao đài
    [SerializeField] private Color pathColor = new Color(0.9f, 0.3f, 0.2f, 0.3f);    // Đỏ nhạt - Ô kẻ địch đi
    [SerializeField] private Color hoverColor = new Color(1.0f, 0.9f, 0.2f, 0.6f);   // Vàng - Khi rê chuột qua

    public GridNode Node { get; private set; }
    private Color originalColor;

    // Khởi tạo Tile với dữ liệu Node
    public void Setup(GridNode node)
    {
        this.Node = node;
        UpdateVisualByTileType();
    }

    // Cập nhật màu sắc dựa trên TileType
    public void UpdateVisualByTileType()
    {
        switch (Node.tileType)
        {
            case TileType.Melee:
                originalColor = meleeColor;
                break;
            case TileType.Ranged:
                originalColor = rangedColor;
                break;
            case TileType.Path:
            case TileType.Blocked:
                originalColor = pathColor;
                break;
        }
        SetColor(originalColor);
    }

    // Đổi màu khi hover chuột
    public void SetHover(bool isHovered)
    {
        if (isHovered)
        {
            SetColor(hoverColor);
        }
        else
        {
            SetColor(originalColor);
        }
    }

    private void SetColor(Color color)
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = color;
        }
    }
}