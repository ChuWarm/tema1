using UnityEngine;

public class BossRoomState : IRoomState
{
    private EnemyBase _boss;
    
    public void Enter(RoomEventProcessor processor)
    {
        DataManager dataManager = Object.FindObjectOfType<DataManager>();
        _boss = EnemyFactory.SpawnEnemy(processor.GetComponent<RoomEventHolder>(),
            dataManager.GetEnemyData("boss_enemy"));
    }

    public void Update(RoomEventProcessor processor)
    {
        if (_boss == null)
        {
            processor.MarkRoomCleared();
            processor.SetState(null);
        }
    }
}
