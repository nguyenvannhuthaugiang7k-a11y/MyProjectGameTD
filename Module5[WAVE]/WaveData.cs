using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemySpawnGroup
{
    public GameObject enemyPrefab; // Prefab loại quái (Ví dụ: Slug, Defense Enemy)
    public int count;               // Số lượng quái loại này
    public float spawnRate;         // Tốc độ sinh (Khoảng cách giữa mỗi con, ví dụ: 1.5 giây/con)
    public float delayBeforeGroup;  // Thời gian chờ trước khi bắt đầu nhóm này
}

[CreateAssetMenu(fileName = "NewWaveData", menuName = "Arknights/Wave Data")]
public class WaveData : ScriptableObject
{
    [Header("Cấu hình Đợt Quái")]
    public List<EnemySpawnGroup> enemyGroups = new List<EnemySpawnGroup>();
}