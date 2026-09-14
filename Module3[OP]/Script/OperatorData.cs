using System.Collections.Generic;
using UnityEngine;

public enum OperatorClass
{
    Melee,
    Defender,
    Ranger,
    Caster,
    Medic,
    Supporter
}

[CreateAssetMenu(fileName = "NewOperatorData", menuName = "Arknights/Operator Data")]
public class OperatorData : ScriptableObject
{
    [Header("Basic Info")]
    public string operatorName = "New Operator";

    public int dpCost = 10;

    public float redeployCooldown = 15f;

    [Header("Operator Class")]
    public OperatorClass operatorClass = OperatorClass.Melee;

    [Header("Deployment Terrain")]
    public TileType allowedTileType = TileType.LowGround;

    [Header("Visual & Prefab")]
    public GameObject operatorPrefab;

    public Sprite icon;

    [Header("Chỉ số Block (Cản quái)")]
    public int blockCount = 2;

    [Header("Attack Range Configuration")]
    public List<Vector2Int> relativeAttackRange = new List<Vector2Int>()
    {
        new Vector2Int(0, 0),
        new Vector2Int(0, 1)
    };
}