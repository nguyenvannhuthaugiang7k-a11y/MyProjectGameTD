using UnityEngine;

public class GridInputManager : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private LayerMask tileLayerMask; // Layer để lọc chỉ Raycast trúng Ô bản đồ

    private Camera mainCamera;
    private TileVisual currentHoveredTile;

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
        if (Input.GetMouseButtonDown(0) && currentHoveredTile != null)
        {
            GridNode node = currentHoveredTile.Node;
        
            // Thử đặt nhân vật đang chọn vào ô được click
            if (PlacementManager.Instance != null)
            {
            PlacementManager.Instance.TryPlaceOperator(node);
            }
        }
    }
}