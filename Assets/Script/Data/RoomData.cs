using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/RoomData")]
public class RoomData : ScriptableObject //, IGameData
{
    public List<EnemyIDCountPair> spawnEnemys;

    // public 



    [System.Serializable]
    public struct EnemyIDCountPair
    {
        public string enemyID;
        public int spawnCount;
    }
}
