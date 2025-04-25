using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject player;
    
    private Transform _spawnPoint;
    
    public void InstantiatePlayer()
    {
        var newSpawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        var playerObj = Instantiate(player, newSpawnPoint.position, Quaternion.identity);
    }

    public GameObject FindPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");

        return player;
    }
}
