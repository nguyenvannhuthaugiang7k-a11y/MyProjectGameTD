using UnityEngine;

public class TileVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Color Settings")]
    [SerializeField] private Color meleeColor = new Color(0.8f, 0.8f, 0.8f, 0.3f);
    [SerializeField] private Color rangedColor = new Color(0.2f, 0.6f, 1.0f, 0.3f);
    [SerializeField] private Color pathColor = new Color(0.9f, 0.3f, 0.2f, 0.3f);
    [SerializeField] private Color hoverColor = new Color(1.0f, 0.9f, 0.2f, 0.6f);

    public GridNode Node { get; private set; }

    private Color originalColor;

    // Trạng thái riêng
    private bool isMouseHovered = false;
    private bool isRangeHighlighted = false;

    public void Setup(GridNode node)
    {
        this.Node = node;
        UpdateVisualByTileType();
    }

    public void UpdateVisualByTileType()
    {
        switch (Node.tileType)
        {
            case TileType.LowGround:
                originalColor = meleeColor;
                break;

            case TileType.HighGround:
                originalColor = rangedColor;
                break;

            case TileType.Blocked:
                originalColor = pathColor;
                break;
        }

        UpdateVisual();
    }

    // Dùng cho mouse hover
    public void SetHover(bool isHovered)
    {
        isMouseHovered = isHovered;
        UpdateVisual();
    }

    // Dùng cho range highlight
    public void SetRangeHighlight(bool isHighlighted)
    {
        isRangeHighlighted = isHighlighted;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (isRangeHighlighted || isMouseHovered)
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