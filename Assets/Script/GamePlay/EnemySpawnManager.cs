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
    public event Action<Enemy> OnEnemySpawned;

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
                    // Debug.LogError($"Normal Room Enemy at index {i} has null prefab!");
                }
            }
        }

        if (eliteRoomEnemies != null)
        {
            for (int i = 0; i < eliteRoomEnemies.Count; i++)
            {
                if (eliteRoomEnemies[i]?.enemyPrefab == null)
                {
                    // Debug.LogError($"Elite Room Enemy at index {i} has null prefab!");
                }
            }
        }
    }

    public List<Enemy> SpawnEnemiesForRoom(RoomType roomType, Transform roomTransform)
    {
        if (roomTransform == null)
        {
            // Debug.LogError($"[스폰매니저] SpawnEnemiesForRoom: roomTransform이 null입니다 (방 타입: {roomType})");
            return new List<Enemy>();
        }

        ClearExistingEnemies();

        if (roomType == RoomType.Spawn)
        {
            // Debug.Log($"[스폰매니저] 스폰 방({roomTransform.name})은 적 스폰을 건너뜁니다.");
            return new List<Enemy>();
        }

        int enemyCount = Random.Range(minEnemyCount, maxEnemyCount + 1);
        List<EnemySpawnInfo> availableEnemies = GetEnemiesForRoomType(roomType);
        List<Enemy> spawnedEnemyComponents = new List<Enemy>();

        if (availableEnemies == null || availableEnemies.Count == 0)
        {
            // Debug.LogError($"[스폰매니저] {gameObject.name}에 {roomType} 타입을 위한 적이 설정되지 않았거나 리스트가 비어있습니다.");
            return new List<Enemy>();
        }
        // Debug.Log($"[스폰매니저] {roomTransform.name}({roomType})에 {enemyCount}마리의 적 스폰 시도. 사용 가능 적 종류: {availableEnemies.Count}");

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition(roomTransform);
            EnemySpawnInfo selectedEnemy = SelectRandomEnemy(availableEnemies);
            
            if (selectedEnemy == null)
            {
                // Debug.LogError($"[스폰매니저] {roomType} 타입을 위한 적 선택 실패.");
                continue;
            }

            if (selectedEnemy.enemyPrefab == null)
            {
                // Debug.LogError($"[스폰매니저] 선택된 적 프리팹이 null입니다 ({roomType}). 프리팹 이름: {(selectedEnemy.enemyPrefab != null ? selectedEnemy.enemyPrefab.name : "NULL")}");
                continue;
            }
            // Debug.Log($"[스폰매니저] 스폰 대상: {selectedEnemy.enemyPrefab.name} at {spawnPosition}");
            GameObject enemy = Instantiate(selectedEnemy.enemyPrefab, spawnPosition, Quaternion.identity, roomTransform);
            if (enemy == null)
            {
                // Debug.LogError($"[스폰매니저] 적 프리팹 인스턴스화 실패: {selectedEnemy.enemyPrefab.name}");
                continue;
            }

            spawnedEnemies.Add(enemy);
            
            if (enemy.TryGetComponent<Enemy>(out var enemyBase))
            {
                spawnedEnemyComponents.Add(enemyBase);
                OnEnemySpawned?.Invoke(enemyBase); // 이 이벤트는 CombatRoomBaseState에서 직접 사용하지 않음
                // Debug.Log($"[스폰매니저] {enemy.name} (Enemy 컴포넌트 있음) 스폰 완료 및 리스트에 추가됨. 현재 spawnedEnemyComponents: {spawnedEnemyComponents.Count}");
            }
            else
            {
                // Debug.LogError($"[스폰매니저] 스폰된 적 {enemy.name}에 Enemy 컴포넌트가 없습니다.");
            }
        }
        // Debug.Log($"[스폰매니저] 최종 스폰된 적 (Enemy 컴포넌트 기준): {spawnedEnemyComponents.Count}마리.");
        return spawnedEnemyComponents;
    }

    private List<EnemySpawnInfo> GetEnemiesForRoomType(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Normal:
                return normalRoomEnemies;
            case RoomType.Normal2:
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