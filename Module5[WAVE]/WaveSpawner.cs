using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Tham chiếu")]
    [SerializeField] private EnemyPath enemyPath;
    [SerializeField] private List<WaveData> waveList = new List<WaveData>();

    [Header("Cấu hình Thời gian")]
    [SerializeField] private float timeBetweenWaves = 5f;

    private int currentWaveIndex = 0;
    private int totalEnemiesInCurrentWave = 0;
    private int activeEnemiesCount = 0;

    private void Start()
    {
        if (waveList.Count > 0)
        {
            StartCoroutine(SpawnWaveRoutine());
        }
        else
        {
            Debug.LogWarning(
                "[WaveSpawner] Chưa gán WaveData nào vào danh sách!"
            );
        }
    }

    private IEnumerator SpawnWaveRoutine()
    {
        while (currentWaveIndex < waveList.Count)
        {
            WaveData currentWave = waveList[currentWaveIndex];

            Debug.Log(
                $"<color=yellow>=== BẮT ĐẦU WAVE " +
                $"{currentWaveIndex + 1}/{waveList.Count} ===</color>"
            );

            // Đếm tổng số quái của Wave hiện tại
            totalEnemiesInCurrentWave = 0;

            foreach (var group in currentWave.enemyGroups)
            {
                totalEnemiesInCurrentWave += group.count;
            }

            activeEnemiesCount = totalEnemiesInCurrentWave;

            Debug.Log(
                $"[WaveSpawner] Wave {currentWaveIndex + 1} có " +
                $"{activeEnemiesCount} Enemy."
            );

            // Spawn từng nhóm quái
            foreach (var group in currentWave.enemyGroups)
            {
                yield return new WaitForSeconds(
                    group.delayBeforeGroup
                );

                for (int i = 0; i < group.count; i++)
                {
                    SpawnEnemy(group.enemyPrefab);

                    yield return new WaitForSeconds(
                        group.spawnRate
                    );
                }
            }

            // Chờ toàn bộ Enemy của Wave biến mất
            yield return new WaitUntil(
                () => activeEnemiesCount <= 0
            );

            Debug.Log(
                $"<color=green>=== HOÀN THÀNH WAVE " +
                $"{currentWaveIndex + 1} ===</color>"
            );

            currentWaveIndex++;

            if (currentWaveIndex < waveList.Count)
            {
                Debug.Log(
                    $"[WaveSpawner] Chờ {timeBetweenWaves} giây " +
                    $"trước Wave {currentWaveIndex + 1}."
                );

                yield return new WaitForSeconds(
                    timeBetweenWaves
                );
            }
        }

        // Hoàn thành toàn bộ Wave
        if (GameManager.Instance != null &&
            GameManager.Instance.CurrentState == GameState.Gameplay)
        {
            GameManager.Instance.TriggerGameWin();
        }
    }

    private void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning(
                "[WaveSpawner] Enemy Prefab chưa được gán!"
            );

            return;
        }

        if (enemyPath == null)
        {
            Debug.LogWarning(
                "[WaveSpawner] EnemyPath chưa được gán!"
            );

            return;
        }

        if (enemyPath.waypoints == null ||
            enemyPath.waypoints.Count == 0)
        {
            Debug.LogWarning(
                "[WaveSpawner] EnemyPath chưa có Waypoint!"
            );

            return;
        }

        // Spawn tại Waypoint đầu tiên (RedBox)
        Vector3 spawnPosition =
            enemyPath.waypoints[0].position;

        GameObject enemyObj = Instantiate(
            prefab,
            spawnPosition,
            Quaternion.identity
        );

        Enemy enemyScript =
            enemyObj.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            // Đăng ký sự kiện Enemy biến mất
            enemyScript.OnDespawned += HandleEnemyDespawned;

            enemyScript.Initialize(enemyPath);
        }
        else
        {
            Debug.LogWarning(
                $"[WaveSpawner] Prefab {prefab.name} " +
                $"không có Enemy component!"
            );
        }
    }

    private void HandleEnemyDespawned(Enemy enemy)
    {
        activeEnemiesCount--;

        activeEnemiesCount =
            Mathf.Max(0, activeEnemiesCount);

        Debug.Log(
            $"[WaveSpawner] Enemy {enemy.name} đã biến mất. " +
            $"Còn lại: {activeEnemiesCount}"
        );
    }
}