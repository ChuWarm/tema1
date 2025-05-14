using Script.Characters;
using UnityEngine;

public class PlayerStateAttack : IPlayerState
{
    private IPlayerAttackBehavior _attackBehavior;
    private PlayerController _player;
    
    public void EnterState(PlayerController playerController)
    {
        _player = playerController;
        _attackBehavior = _player.GetAttackBehavior();
        _attackBehavior.Enter(_player);
    }

    public void UpdateState()
    {
        if (_player.IsAttacking)
        {
            _attackBehavior?.Update();
        }
    }

    public void ExitState()
    {
        _attackBehavior?.Exit();
        _player = null;
    }
}