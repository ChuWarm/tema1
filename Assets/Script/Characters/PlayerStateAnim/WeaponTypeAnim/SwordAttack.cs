using UnityEngine;
using Script.Characters;

public class SwordAttack : IPlayerAttackBehavior
{
    private PlayerController _playerController;
    
    public void Enter(PlayerController player)
    {
        _playerController = player;
        _playerController.TriggerAttack();
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        { 
            _playerController.TriggerAttack();
        }
    }

    public void Exit()
    {
        _playerController = null;
    }
}
