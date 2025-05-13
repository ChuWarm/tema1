using UnityEngine;
using System.Collections.Generic;
using Script.Characters;

public class EnemySpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        public float spawnWeight = 1f;
    }

    [Header("Enemy Spawn Settings")]
    [SerializeField] private List<EnemySpawnInfo> normalRoomEnemies;
    [SerializeField] private List<EnemySpawnInfo> eliteRoomEnemies;
    [SerializeField] private float minEnemyDistance = 2f;
    [SerializeField] private float maxEnemyDistance = 4f;
    [SerializeField] private int minEnemyCount = 3;
    [SerializeField] private int maxEnemyCount = 5;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public void SpawnEnemiesForRoom(RoomType roomType, Transform roomTransform)
    {
        ClearExistingEnemies();

        if (roomType == RoomType.Spawn)
            return;

        int enemyCount = Random.Range(minEnemyCount, maxEnemyCount + 1);
        List<EnemySpawnInfo> availableEnemies = GetEnemiesForRoomType(roomType);

        if (availableEnemies.Count == 0)
        {
            Debug.LogWarning($"No enemies configured for room type: {roomType}");
            return;
        }

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition(roomTransform);
            EnemySpawnInfo selectedEnemy = SelectRandomEnemy(availableEnemies);
            
            if (selectedEnemy != null && selectedEnemy.enemyPrefab != null)
            {
                GameObject enemy = Instantiate(selectedEnemy.enemyPrefab, spawnPosition, Quaternion.identity, roomTransform);
                spawnedEnemies.Add(enemy);
            }
        }
    }

    private List<EnemySpawnInfo> GetEnemiesForRoomType(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Normal:
                return normalRoomEnemies;
            case RoomType.Elite:
                return eliteRoomEnemies;
            default:
                return new List<EnemySpawnInfo>();
        }
    }

    private EnemySpawnInfo SelectRandomEnemy(List<EnemySpawnInfo> enemies)
    {
        float totalWeight = 0f;
        foreach (var enemy in enemies)
        {
            totalWeight += enemy.spawnWeight;
        }

        float random = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var enemy in enemies)
        {
            currentWeight += enemy.spawnWeight;
            if (random <= currentWeight)
            {
                return enemy;
            }
        }

        return enemies[0];
    }

    private Vector3 GetRandomSpawnPosition(Transform roomTransform)
    {
        // 방의 중앙에서 일정 거리 내에서 랜덤한 위치 선택
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(minEnemyDistance, maxEnemyDistance);
        
        Vector3 offset = new Vector3(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomDistance,
            0f,
            Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomDistance
        );

        return roomTransform.position + offset;
    }

    private void ClearExistingEnemies()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
    }

    private void OnDestroy()
    {
        ClearExistingEnemies();
    }
} 