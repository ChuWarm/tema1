using UnityEngine;

public class PlayerStateAttack : IPlayerState
{
    private PlayerController _player;
    
    public void EnterState(PlayerController playerController)
    {
        _player = playerController;
        _player.currentLookMode = LookMode.None;
    }

    public void UpdateState()
    { 
        if (Input.GetButtonDown("Fire1"))
        {
            _player.LookAtMouse();
            Debug.Log("공격!");
            return;
        }
        
        Vector3 input = new(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        _player.Move(input);
        
        if (input.magnitude < 0.1f)
        {
            _player.SetState(PlayerState.Idle);
        }
    }

    public void ExitState()
    {
        _player = null;
    }
}
