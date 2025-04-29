using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
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
