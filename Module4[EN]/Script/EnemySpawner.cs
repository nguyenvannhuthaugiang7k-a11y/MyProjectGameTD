using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private EnemyPath enemyPath;

    [Header("Spawn Settings")]
    [SerializeField, Min(0.1f)]
    private float spawnInterval = 2.5f;

    [SerializeField, Min(1)]
    private int maxEnemies = 10;

    [Header("Control")]
    [SerializeField]
    private bool spawnOnStart = true;

    private int currentEnemyCount;
    private Coroutine spawnCoroutine;

    private void Start()
    {
        if (spawnOnStart)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (spawnCoroutine != null)
        {
            return;
        }

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine == null)
        {
            return;
        }

        StopCoroutine(spawnCoroutine);
        spawnCoroutine = null;
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (CanSpawn())
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private bool CanSpawn()
    {
        return enemyPrefab != null
            && enemyPath != null
            && currentEnemyCount < maxEnemies;
    }

    private void SpawnEnemy()
    {
        GameObject enemyObject = Instantiate(
            enemyPrefab,
            enemyPath.GetStartPosition(),
            Quaternion.identity
        );

        currentEnemyCount++;
    }

    private void OnDestroy()
    {
        StopSpawning();
    }
}