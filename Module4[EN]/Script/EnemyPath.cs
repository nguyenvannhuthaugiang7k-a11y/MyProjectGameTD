using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    [Header("Danh sách Waypoints (Từ Red Box đến Blue Box)")]
    public List<Transform> waypoints = new List<Transform>();

    public Vector3 GetStartPosition()
    {
        if (waypoints == null || waypoints.Count == 0 || waypoints[0] == null)
        {
            Debug.LogWarning($"[{nameof(EnemyPath)}] Chưa có Waypoint bắt đầu.");
            return transform.position;
        }

    return waypoints[0].position;
    }

    // Tính tổng quãng đường còn lại mà quái phải đi để tới Cổng Xanh
    public float GetRemainingDistance(int currentWaypointIndex, Vector3 currentPosition)
    {
        if (waypoints == null || waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count)
            return 0f;

        // 1. Khoảng cách từ quái tới Waypoint tiếp theo
        float distance = Vector3.Distance(currentPosition, waypoints[currentWaypointIndex].position);

        // 2. Cộng dồn khoảng cách giữa các Waypoint còn lại
        for (int i = currentWaypointIndex; i < waypoints.Count - 1; i++)
        {
            distance += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        }

        return distance;
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count < 2) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}