using System.Collections;
using System.Collections.Generic;
using Script.Characters;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject gameOverPanel;

    public GamePlayLogic gamePlayLogic;
    private Transform _spawnPoint;

    public Transform GetPlayer => player?.transform;

    private void Start()
    {
        GameEventBus.Subscribe<NewGameStart>(OnNewGameStart);
        GameEventBus.Subscribe<PlayerDeath>(OnPlayerDeath);
    }

    public void OnNewGameStart(NewGameStart e)
    {
        Debug.Log("새 게임 시작");   
        gamePlayLogic = new GamePlayLogic(SpawnPlayer().GetComponent<PlayerController>());
    }

    void OnPlayerDeath(PlayerDeath e)
    {
        gamePlayLogic = null;
        gameOverPanel.SetActive(true);
    }

    GameObject SpawnPlayer()
    {
        var newSpawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        var playerObj = Instantiate(player, newSpawnPoint.position, Quaternion.identity);
        player = playerObj;
        
        // CameraController.Instance.CameraInit();

        return playerObj;
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