using Script.Characters;
using UnityEngine;
using Script.Characters;

public class GunAttack : IPlayerAttackBehavior
{
    private PlayerController _playerController;
    private float moveSpeedDuringAttack = 5f;
    
    public void Enter(PlayerController player)
    {
        _playerController = player;
        _playerController.TriggerAttack();
    }

    public void Update()
    {
    }

    public void Exit()
    {
        _playerController = null;
    }
}
