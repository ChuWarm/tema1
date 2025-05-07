using Script.Characters;
using UnityEngine;

public class PlayerStateAttack : IPlayerState
{
    private PlayerController _player;
    
    public void EnterState(PlayerController playerController)
    {
        _player = playerController;
    }

    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        _player = null;
    }
}
