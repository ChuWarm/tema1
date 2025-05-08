using Script.Characters;
using UnityEngine;

public class PlayerStateAttack : IPlayerState
{
    private IPlayerAttackBehavior _attackBehavior;
    private PlayerController _player;
    
    public void EnterState(Script.Characters.PlayerController playerController)
    {
        _player = playerController;
        _attackBehavior = _player.GetAttackBehavior();
        _attackBehavior.Enter(_player);
    }

    public void UpdateState()
    { 
        _attackBehavior?.Update();
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            _player.SetState(PlayerState.Idle);
        }
    }

    public void ExitState()
    {
        _attackBehavior?.Exit();
        _player = null;
    }
}
