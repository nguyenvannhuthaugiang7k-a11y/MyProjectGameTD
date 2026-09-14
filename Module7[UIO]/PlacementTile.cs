using UnityEngine;

public class PlacementTile : MonoBehaviour
{
    public TileType tileType = TileType.LowGround;
    [HideInInspector] public bool isOccupied = false;
    [HideInInspector] public GameObject currentOperator;

    public Vector3 GetPlacementPosition()
    {
        return transform.position;
    }
}