using UnityEngine;
using System.Collections.Generic;
using Script.Characters;
using System;
using Random = UnityEngine.Random;

public class EnemySpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        public float spawnWeight = 1f;
    }

    [Header("Enemy Spawn Settings")]
    [SerializeField] private List<EnemySpawnInfo> normalRoomEnemies = new List<EnemySpawnInfo>();
    [SerializeField] private List<EnemySpawnInfo> eliteRoomEnemies = new List<EnemySpawnInfo>();
    [SerializeField] private float minEnemyDistance = 2f;
    [SerializeField] private float maxEnemyDistance = 4f;
    [SerializeField] private int minEnemyCount = 3;
    [SerializeField] private int maxEnemyCount = 5;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    public event Action<EnemyBase> OnEnemySpawned;

    private void Awake()
    {
        // 리스트가 null인 경우 초기화
        if (normalRoomEnemies == null) normalRoomEnemies = new List<EnemySpawnInfo>();
        if (eliteRoomEnemies == null) eliteRoomEnemies = new List<EnemySpawnInfo>();
        
        // 설정된 적 프리팹 확인
        ValidateEnemyPrefabs();
    }

    private void ValidateEnemyPrefabs()
    {
        if (normalRoomEnemies != null)
        {
            for (int i = 0; i < normalRoomEnemies.Count; i++)
            {
                if (normalRoomEnemies[i]?.enemyPrefab == null)
                {
                    Debug.LogError($"Normal Room Enemy at index {i} has null prefab!");
                }
            }
        }

        if (eliteRoomEnemies != null)
        {
            for (int i = 0; i < eliteRoomEnemies.Count; i++)
            {
                if (eliteRoomEnemies[i]?.enemyPrefab == null)
                {
                    Debug.LogError($"Elite Room Enemy at index {i} has null prefab!");
                }
            }
        }
    }

    public void SpawnEnemiesForRoom(RoomType roomType, Transform roomTransform)
    {
        if (roomTransform == null)
        {
            Debug.LogError($"SpawnEnemiesForRoom: roomTransform is null for room type {roomType}");
            return;
        }

        ClearExistingEnemies();

        if (roomType == RoomType.Spawn)
        {
            Debug.Log($"Skipping enemy spawn for Spawn room");
            return;
        }

        int enemyCount = Random.Range(minEnemyCount, maxEnemyCount + 1);
        List<EnemySpawnInfo> availableEnemies = GetEnemiesForRoomType(roomType);

        if (availableEnemies == null || availableEnemies.Count == 0)
        {
            Debug.LogError($"No enemies configured for room type: {roomType} on {gameObject.name}");
            return;
        }

        Debug.Log($"Spawning {enemyCount} enemies for room type {roomType}");

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition(roomTransform);
            EnemySpawnInfo selectedEnemy = SelectRandomEnemy(availableEnemies);
            
            if (selectedEnemy == null)
            {
                Debug.LogError($"Failed to select random enemy for room type {roomType}");
                continue;
            }

            if (selectedEnemy.enemyPrefab == null)
            {
                Debug.LogError($"Selected enemy prefab is null for room type {roomType}");
                continue;
            }

            GameObject enemy = Instantiate(selectedEnemy.enemyPrefab, spawnPosition, Quaternion.identity, roomTransform);
            if (enemy == null)
            {
                Debug.LogError($"Failed to instantiate enemy prefab");
                continue;
            }

            spawnedEnemies.Add(enemy);
            
            if (enemy.TryGetComponent<EnemyBase>(out var enemyBase))
            {
                OnEnemySpawned?.Invoke(enemyBase);
            }
            else
            {
                Debug.LogError($"Spawned enemy {enemy.name} does not have EnemyBase component");
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