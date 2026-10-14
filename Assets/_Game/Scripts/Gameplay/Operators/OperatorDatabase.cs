using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "OperatorDatabase",
    menuName = "Arknights/Operator Database"
)]
public class OperatorDatabase : ScriptableObject
{
    [Header("Danh sách Operator")]
    [SerializeField] private List<OperatorData> operators =
        new List<OperatorData>();

    public IReadOnlyList<OperatorData> Operators
    {
        get { return operators; }
    }
}