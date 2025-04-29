using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
    [SerializeField] private GameObject player;

    public GamePlayLogic gamePlayLogic;

    private Transform _spawnPoint;

    private void Start()
    {
        gamePlayLogic = new GamePlayLogic(player.GetComponent<PlayerController>());
    }

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
