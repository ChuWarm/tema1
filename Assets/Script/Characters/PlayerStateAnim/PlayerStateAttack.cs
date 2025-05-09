using Script.Characters;
using UnityEngine;

public class PlayerStateAttack : IPlayerState
{
    private IPlayerAttackBehavior _attackBehavior;
    private PlayerController _player;
    public bool IsAttacking { get; set; }
    
    public void EnterState(Script.Characters.PlayerController playerController)
    {
        _player = playerController;
        _attackBehavior = _player.GetAttackBehavior();
        _attackBehavior.Enter(_player);
    }

    public void UpdateState()
    {
        if (IsAttacking)
        {
            _attackBehavior?.Update();
            return;
        }
        // if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0 && !IsAttacking)
        // {
        //     _player.SetState(PlayerState.Idle);
        // }
    }

    public void ExitState()
    {
        _attackBehavior?.Exit();
        _player = null;
    }
}
