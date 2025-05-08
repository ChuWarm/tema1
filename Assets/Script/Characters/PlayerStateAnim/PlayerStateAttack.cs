using UnityEngine;

public class PlayerStateAttack : IPlayerState
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    private PlayerController _player;
    
    public void EnterState(PlayerController playerController)
    {
        _player = playerController;
        _player.Animator.SetTrigger(Attack);
    }

    public void UpdateState()
    { 
        if (Input.GetButtonDown("Fire1"))
        {
            _player.Animator.SetTrigger(Attack);
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
