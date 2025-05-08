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
        _playerController.currentLookMode = LookMode.Mouse;
        _playerController.TirggerAttack();
    }

    public void Update()
    {
    }

    public void Exit()
    {
        _playerController = null;
    }
}
