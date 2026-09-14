using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Dimensions")]
    [SerializeField] private int width = 8;        // Số ô theo trục X
    [SerializeField] private int height = 6;       // Số ô theo trục Z
    [SerializeField] private float cellSize = 1f;  // Kích thước mỗi ô (mét)

    [Header("Tile Prefab & Visual")]
    [SerializeField] private GameObject tilePrefab; // Prefab 3D đại diện cho từng ô trên bàn cờ

    [Header("Visual Debug")]
    [SerializeField] private bool showGizmos = true;

    // Các ma trận lưu trữ dữ liệu
    private GridNode[,] gridArray;
    private TileVisual[,] visualArray;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        GenerateGrid();
    }

    #region GRID GENERATION & POSITIONING

    private void GenerateGrid()
    {
        gridArray = new GridNode[width, height];
        visualArray = new TileVisual[width, height];

        int tileLayer = LayerMask.NameToLayer("Tile");

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 worldPos = GetWorldPosition(x, z);

                // =========================================================
                // 1. XÁC ĐỊNH LOẠI ĐỊA HÌNH VÀ ENEMY PATH
                // =========================================================

                TileType currentType;
                bool isEnemyPath;

                // Hai hàng giữa (z = 2 và z = 3)
                // là đường đi của Enemy.
                //
                // Blue Path:
                // - Low Ground
                // - Enemy có thể đi qua
                // - Melee có thể đứng tại đây
                // - Ranged/Caster không được đặt tại đây
                if (z == 2 || z == 3)
                {
                    currentType = TileType.LowGround;
                    isEnemyPath = true;
                }
                else
                {
                    // Các ô còn lại hiện tại là High Ground.
                    //
                    // High Ground:
                    // - Ranged
                    // - Caster
                    // - Sau này Medic
                    // - Sau này Supporter
                    currentType = TileType.HighGround;
                    isEnemyPath = false;
                }

                // =========================================================
                // 2. KHỞI TẠO GRID NODE
                // =========================================================

                gridArray[x, z] = new GridNode(
                    x,
                    z,
                    worldPos,
                    currentType
                );

                // Ghi nhận ô này có phải Enemy Path hay không.
                gridArray[x, z].isEnemyPath = isEnemyPath;

                // =========================================================
                // 3. SINH TILE 3D
                // =========================================================

                if (tilePrefab != null)
                {
                    GameObject tileObj = Instantiate(
                        tilePrefab,
                        worldPos,
                        Quaternion.identity,
                        transform
                    );

                    // Tự động gán Layer "Tile"
                    if (tileLayer != -1)
                    {
                        tileObj.layer = tileLayer;
                    }

                    // =====================================================
                    // 4. ĐỒNG BỘ PLACEMENT TILE
                    // =====================================================

                    PlacementTile placementTile =
                        tileObj.GetComponent<PlacementTile>();

                    if (placementTile == null)
                    {
                        placementTile =
                            tileObj.AddComponent<PlacementTile>();
                    }

                    placementTile.tileType = currentType;

                    // =====================================================
                    // 5. ĐỒNG BỘ TILE VISUAL
                    // =====================================================

                    TileVisual visual =
                        tileObj.GetComponent<TileVisual>();

                    if (visual != null)
                    {
                        visual.Setup(gridArray[x, z]);
                        visualArray[x, z] = visual;
                    }
                }
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(
            x * cellSize + cellSize * 0.5f,
            0,
            z * cellSize + cellSize * 0.5f
        ) + transform.position;
    }

    public GridNode GetNodeFromWorldPosition(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - transform.position;

        int x = Mathf.FloorToInt(localPos.x / cellSize);
        int z = Mathf.FloorToInt(localPos.z / cellSize);

        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            return gridArray[x, z];
        }

        return null;
    }

    public TileVisual GetTileVisual(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            return visualArray[x, z];
        }

        return null;
    }

    #endregion

    #region ATTACK RANGE & HIGHLIGHT SYSTEM

    public List<GridNode> GetNodesInRange(
        Vector2Int originCoord,
        List<Vector2Int> relativeRange,
        Direction direction)
    {
        List<GridNode> resultNodes = new List<GridNode>();

        if (relativeRange == null)
        {
            return resultNodes;
        }

        foreach (Vector2Int relPos in relativeRange)
        {
            Vector2Int rotatedPos =
                RotateRelativePosition(relPos, direction);

            int targetX = originCoord.x + rotatedPos.x;
            int targetZ = originCoord.y + rotatedPos.y;

            if (targetX >= 0 &&
                targetX < width &&
                targetZ >= 0 &&
                targetZ < height)
            {
                resultNodes.Add(gridArray[targetX, targetZ]);
            }
        }

        return resultNodes;
    }

    private Vector2Int RotateRelativePosition(
        Vector2Int relative,
        Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return relative;

            case Direction.Down:
                return new Vector2Int(
                    -relative.x,
                    -relative.y
                );

            case Direction.Left:
                return new Vector2Int(
                    -relative.y,
                    relative.x
                );

            case Direction.Right:
                return new Vector2Int(
                    relative.y,
                    -relative.x
                );

            default:
                return relative;
        }
    }

    public void HighlightRangeNodes(
        List<GridNode> nodes,
        bool isHighlight)
    {
        if (nodes == null)
        {
            return;
        }

        foreach (GridNode node in nodes)
        {
            TileVisual visual =
                GetTileVisual(node.xIndex, node.zIndex);

            if (visual != null)
            {
                visual.SetRangeHighlight(isHighlight);
            }
        }
    }

    #endregion

    #region EDITOR DEBUG

    private void OnDrawGizmos()
    {
        if (!showGizmos)
        {
            return;
        }

        Gizmos.color = Color.white;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 center =
                    GetWorldPosition(x, z);

                Gizmos.DrawWireCube(
                    center,
                    new Vector3(
                        cellSize * 0.95f,
                        0.1f,
                        cellSize * 0.95f
                    )
                );
            }
        }
    }

    #endregion
}