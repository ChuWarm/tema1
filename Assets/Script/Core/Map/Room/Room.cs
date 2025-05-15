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
    private bool _playerHasEntered;
    
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
        if (_playerHasEntered) return;

        float distance = Vector3.Distance(_playerTransform.position, transform.position);
        if (distance < enterThreshold)
        {
            _eventProcessor.OnPlayerEnterRoom();
            _playerHasEntered = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        _playerTransform = null;
        _playerHasEntered = false;
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
        string roomName = gameObject.name;
        Debug.Log($"[{roomName}] MarkRoomAsCleared 호출됨. 현재 IsCleared: {IsCleared}");

        if (_mapData == null)
        {
            Debug.LogError($"[{roomName}] MarkRoomAsCleared: _mapData가 null이므로 중단합니다.");
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
        string roomName = gameObject.name;
        Debug.Log($"[{roomName}] UpdateDoorsAfterClear() 호출됨. RoomType: {RoomType}");

        if (_mapData == null)
        {
             Debug.LogError($"[{roomName}] UpdateDoorsAfterClear: _mapData가 null이므로 중단합니다.");
             return;
        }

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
        string roomName = gameObject.name;
        Debug.Log($"[{roomName}] UpdateConnectedDoorsBasedOnMapData() 호출됨.");

        if (_doors == null) { Debug.LogWarning($"[{roomName}] _doors가 null입니다."); return; }
        if (_mapData == null) { Debug.LogWarning($"[{roomName}] _mapData가 null입니다."); return; }
        if (_mapData.doors == null) { Debug.LogWarning($"[{roomName}] _mapData.doors가 null입니다."); return; }

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
        string roomName = gameObject.name;
        Debug.Log($"[{roomName}] UpdateNeighboringRoomDoors() 호출됨.");

        if (_mapData == null) { Debug.LogWarning($"[{roomName}] _mapData가 null입니다."); return; }
        if (MapGenerator.Instance == null) { Debug.LogWarning($"[{roomName}] MapGenerator.Instance가 null입니다."); return; }

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
            else
            {
                Debug.LogWarning($"[{roomName}] UpdateNeighboringRoomDoors - 이웃 방 {neighborPos}를 찾을 수 없습니다.");
            }
        }
    }

    public void CloseAllDoors()
    {
        string roomName = gameObject.name;
        if (_doors == null) 
        {
            Debug.LogWarning($"[{roomName}] CloseAllDoors: _doors가 null입니다.");
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
        string roomName = gameObject.name;
        Debug.Log($"[{roomName}] OpenAllDoors() 호출됨.");
        if (_doors == null) 
        {
            Debug.LogWarning($"[{roomName}] OpenAllDoors: _doors가 null입니다.");
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
        string roomName = gameObject.name;
        Debug.Log($"[{roomName}] OpenDoorInDirection({directionIndex}) 호출됨.");
        if (_doors != null && directionIndex >= 0 && directionIndex < _doors.Length && _doors[directionIndex] != null)
        {
            var doorToOpen = _doors[directionIndex];
            doorToOpen.Open();
        }
        else
        {
            Debug.LogWarning($"[{roomName}] OpenDoorInDirection({directionIndex}) - 유효하지 않은 문 인덱스 또는 문이 null입니다.");
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
