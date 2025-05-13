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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackCancel();
            _playerController.TriggerDash();
            return;
        }
        if (Input.GetButtonDown("Fire1"))
        { 
            _playerController.TriggerAttack();
        }
    }
    
    public void Exit()
    {
        _playerController = null;
    }
    
    private void AttackCancel()
    {
        _playerController.animator.ResetTrigger(PlayerController.AttackAnim);
        _playerController.animator.ResetTrigger(PlayerController.DashAnim);
        _playerController.animator.speed = 1f;
        _playerController.animator.Play(PlayerController.Idle);
        _playerController.IsAttacking = false;
        _playerController.IsAttackMoving = false;
    }
}
