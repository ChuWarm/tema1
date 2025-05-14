using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

public class Room : MonoBehaviour
{
    [SerializeField] private Door doorNorth, doorSouth, doorEast, doorWest;
    
    [Header("진입 설정")]
    [SerializeField] private float enterThreshold = 50f;
    
    private Door[] _doors;
    private Transform _playerTransform;
    private MapData _mapData;
    private RoomPrefabType _roomPrefabType;
    private RoomEventProcessor _eventProcessor;
    
    public RoomType RoomType => _roomPrefabType?.roomType ?? _mapData?.roomType ?? RoomType.Normal;
    public bool IsEventProcessorInitialized => _eventProcessor?.IsInitialized ?? false;
    public bool IsCleared => _mapData?.isCleared ?? false;
    public Vector2Int GridPosition => _mapData?.gridPos ?? Vector2Int.zero;

    public async UniTask WaitForEventProcessorInitializationAsync()
    {
        if (_eventProcessor == null) 
        {
            return;
        }
        await _eventProcessor.InitializationTask;
    }

    private void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        _doors = new[] { doorNorth, doorSouth, doorEast, doorWest };
        _roomPrefabType = GetComponent<RoomPrefabType>();
        _eventProcessor = GetComponent<RoomEventProcessor>();
        
        if (_eventProcessor == null)
        {
            _eventProcessor = gameObject.AddComponent<RoomEventProcessor>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        _playerTransform = other.transform;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || _playerTransform == null) return;
        if (!IsEventProcessorInitialized) return;

        float distance = Vector3.Distance(_playerTransform.position, transform.position);
        if (distance < enterThreshold)
        {
            _eventProcessor.OnPlayerEnterRoom();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        _playerTransform = null;
    }
    
    public void Init(MapData data)
    {
        _mapData = data;
    }

    public void ForcePlayerEnter()
    {
        if (!IsEventProcessorInitialized) 
        {
        }
        _eventProcessor.OnPlayerEnterRoom();
    }

    public void MarkRoomAsCleared()
    {
        if (_mapData == null)
        {
            return;
        }

        if (IsCleared)
        {
            UpdateDoorsAfterClear();
            return;
        }
        
        _mapData.isCleared = true;
        UpdateDoorsAfterClear();
    }

    private void UpdateDoorsAfterClear()
    {
        if (_mapData == null) return;

        if (RoomType == RoomType.Spawn)
        {
            OpenAllDoors();
            return;
        }

        UpdateConnectedDoorsBasedOnMapData();
        UpdateNeighboringRoomDoors();
    }

    private void UpdateConnectedDoorsBasedOnMapData()
    {
        if (_doors == null || _mapData == null || _mapData.doors == null) return;
        for (int i = 0; i < _doors.Length && i < _mapData.doors.Length; i++)
        {
            if (_mapData.doors[i] && _doors[i] != null)
            {
                _doors[i].Open();
            }
        }
    }

    private void UpdateNeighboringRoomDoors()
    {
        if (_mapData == null || MapGenerator.Instance == null) return;
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };
        for (int i = 0; i < directions.Length && i < _mapData.doors.Length; i++)
        {
            if (!_mapData.doors[i]) continue;

            Vector2Int neighborPos = GridPosition + directions[i];
            if (MapGenerator.Instance.TryGetRoom(neighborPos, out Room neighborRoom))
            {
                int oppositeDirection = GetOppositeDirection(i);
                neighborRoom.OpenDoorInDirection(oppositeDirection);
            }
        }
    }

    public void CloseAllDoors()
    {
        if (_doors == null) 
        {
            return;
        }
        foreach (var door in _doors)
        {
            if (door != null) 
            {
                door.Close();
            }
        }
    }

    public void OpenAllDoors()
    {
        if (_doors == null) 
        {
            return;
        }
        foreach (var door in _doors)
        {
            if (door != null) 
            {
                door.Open();
            }
        }
    }

    public void OpenDoorInDirection(int directionIndex)
    {
        if (_doors != null && directionIndex >= 0 && directionIndex < _doors.Length && _doors[directionIndex] != null)
        {
            _doors[directionIndex].Open();
        }
    }

    private int GetOppositeDirection(int directionIndex)
    {
        return directionIndex switch
        {
            0 => 1,
            1 => 0,
            2 => 3,
            3 => 2,
            _ => -1
        };
    }

    public MapData GetMapData() => _mapData;
    public Door[] GetDoors() => _doors;
}
