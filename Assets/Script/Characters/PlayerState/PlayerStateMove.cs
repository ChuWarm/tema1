using UnityEngine;

public class PlayerStateMove : IPlayerState
{
    private PlayerController _playerController;

    public void EnterState(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.currentLookMode = LookMode.Movement;
    }

    public void UpdateState()
    {
        Vector3 input = new(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (input.magnitude > 0.1f)
        {
            _playerController.Move(input);
        }
        else
        {
            _playerController.SetState(PlayerState.Idle);
        }
        
        if (Input.GetButtonDown("Fire1"))
        {
            _playerController.SetState(PlayerState.Attack);
        }
    }

    public void ExitState()
    {
        _playerController = null;
    }
    
    
}
