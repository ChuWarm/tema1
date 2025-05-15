using Script.Characters;
using UnityEngine;

public class PlayerStateHit : IPlayerState
{
    private PlayerController _player;
    
    public void EnterState(Script.Characters.PlayerController playerController)
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
