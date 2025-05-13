using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Room : MonoBehaviour
{
    [SerializeField] private Door doorNorth, doorSouth, doorEast, doorWest;
    
    private Door[] _doors;
    private float _enterThreshold = 50f;
    private bool _entered = false;
    private Transform _playerTransform;
    private MapData _mapData;
    private RoomEventProcessor _eventProcessor;
    
    public RoomType RoomType => _mapData?.roomType ?? RoomType.Normal;

    private void OnEnable()
    {
        _doors = new Door[] { doorNorth, doorSouth, doorEast, doorWest };
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("방 진입 : 플레이어 감지");
        if (!_entered && other.CompareTag("Player"))
        {
            _playerTransform = other.transform;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_entered && other.CompareTag("Player"))
        {
            float distance = Vector3.Distance(_playerTransform.position, transform.position);
            if (distance < _enterThreshold)
            {
                _entered = true;
                CloseAllDoors();
                GetComponent<RoomEventProcessor>()?.OnPlayerEnterRoom();
                Debug.Log($"방 {_mapData.gridPos} 진입 완료, 문 활성화");
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("방 퇴장");
        if (other.CompareTag("Player"))
        {
            _playerTransform = null;
        }
    }
    
    public void Init(MapData data)
    {
        _mapData = data;
    }

    public void ForceEnter()
    {
        _entered = true;
        CloseAllDoors();
        GetComponent<RoomEventProcessor>()?.OnPlayerEnterRoom();
    }

    private void CloseAllDoors()
    {
        foreach (var door in _doors)
        {
            door.Close();
        }
    }

    public void MarkRoomCleared()
    {
        _mapData.isCleared = true;

        // 현재 방 문 열기
        for (int i = 0; i < 4; i++)
        {
            if (_mapData.doors[i]) 
                _doors[i].Open();
        }
        
        // 인접한 방 문 열기
        OpenConnectedNeighborDoors();
        Debug.Log($"방 {_mapData.gridPos} 클리어, 문 비활성화");
    }
    
    private void OpenConnectedNeighborDoors()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left
        };

        for (int i = 0; i < directions.Length; i++)
        {
            if (_mapData.doors[i] == false) continue;

            Vector2Int neighborPos = _mapData.gridPos + directions[i];
            if (MapGenerator.Instance.TryGetRoom(neighborPos, out Room neighborRoom))
            {
                neighborRoom.OpenDoorInDirection(OppositeIndex(i));
                Debug.Log($"인접한 방 문 열기 : {neighborPos} , {neighborRoom}");
                _doors[i].Open();
            }
        }
    }
    
    // 반대 방향 인덱스를 구함
    private int OppositeIndex(int idx)
    {
        return idx switch
        {
            0 => 1,
            1 => 0,
            2 => 3,
            3 => 2,
            _ => -1
        };
    }
    
    public void OpenDoorInDirection(int dirIndex)
    {
        if (_doors[dirIndex] != null)
            _doors[dirIndex].Open();
    }
}
