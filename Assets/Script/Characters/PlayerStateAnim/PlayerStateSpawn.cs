using Script.Characters;

public class PlayerStateSpawn : IPlayerState
{
    private PlayerController _player;
    
    public void EnterState(PlayerController playerController)
    {
        _player = playerController;
        _player.animator.SetTrigger(PlayerController.Spawn);
    }

    public void UpdateState()
    {
    }

    public void ExitState()
    {
        _player = null;
    }
}
