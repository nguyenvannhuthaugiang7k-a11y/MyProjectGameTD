using System.Collections.Generic;
using UnityEngine;
using System;

public class SquadManager : MonoBehaviour
{
    public static SquadManager Instance { get; private set; }

    [Header("Squad Settings")]
    [SerializeField] private int maxSquadSize = 6;

    private List<OperatorData> selectedOperators =
        new List<OperatorData>();

    public event Action OnSquadChanged;

    public IReadOnlyList<OperatorData> SelectedOperators
    {
        get { return selectedOperators; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddOperator(OperatorData data)
    {
        if (data == null)
        {
            Debug.LogWarning(
                "[SquadManager] OperatorData không hợp lệ!"
            );

            return false;
        }

        if (selectedOperators.Count >= maxSquadSize)
        {
            Debug.Log(
                "[SquadManager] Squad đã đủ " +
                maxSquadSize + " Operator!"
            );

            return false;
        }

        if (selectedOperators.Contains(data))
        {
            Debug.Log(
                $"[SquadManager] {data.operatorName} đã có trong Squad!"
            );

            return false;
        }

        selectedOperators.Add(data);
        OnSquadChanged?.Invoke();

        Debug.Log(
            $"[SquadManager] Đã thêm {data.operatorName} vào Squad. " +
            $"({selectedOperators.Count}/{maxSquadSize})"
        );

        return true;
    }

    public bool RemoveOperator(OperatorData data)
    {
        if (data == null)
            return false;

        if (!selectedOperators.Contains(data))
            return false;

        selectedOperators.Remove(data);
        OnSquadChanged?.Invoke();

        Debug.Log(
            $"[SquadManager] Đã xóa {data.operatorName} khỏi Squad. " +
            $"({selectedOperators.Count}/{maxSquadSize})"
        );

        return true;
    }

    public bool IsSelected(OperatorData data)
    {
        if (data == null)
            return false;

        return selectedOperators.Contains(data);
    }

    public bool IsFull()
    {
        return selectedOperators.Count >= maxSquadSize;
    }

    public int GetSelectedCount()
    {
        return selectedOperators.Count;
    }

    public int GetMaxSquadSize()
    {
        return maxSquadSize;
    }

    public void ClearSquad()
    {
        selectedOperators.Clear();
        OnSquadChanged?.Invoke();

        Debug.Log("[SquadManager] Đã xóa toàn bộ Squad.");
    }
}