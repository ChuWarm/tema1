using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject doorNorth, doorSouth, doorEast, doorWest;
    
    private float _enterThreshold = 40f;
    private GameObject[] _doors;
    private MapData _mapData;
    private Transform _playerTransform;
    private bool _entered = false;

    private void OnEnable()
    {
        _doors = new GameObject[] { doorNorth, doorSouth, doorEast, doorWest };
    }

    private void Update()
    {
        if (_entered && Input.GetKeyDown(KeyCode.C))
        {
            MarkRoomCleared();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
                ActivateDoors();
                Debug.Log($"방 {_mapData.gridPos} 진입 완료, 문 활성화");
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerTransform = null;
        }
    }
    
    public void Init(MapData data)
    {
        _mapData = data;
        
        foreach (var door in _doors) 
            door.SetActive(false);
    }

    private void ActivateDoors()
    {
        for (int i = 0; i < 4; i++)
        {
            if (_mapData.doors[i] && !_mapData.isCleared)
                _doors[i].SetActive(true);
        }
    }

    public void MarkRoomCleared()
    {
        _mapData.isCleared = true;

        for (int i = 0; i < 4; i++)
        {
            if (_mapData.doors[i] && _mapData.isCleared)
            {
                _doors[i].SetActive(false);
            }
        }
        
        Debug.Log($"방 {_mapData.gridPos} 클리어, 문 비활성화");
    }
}
