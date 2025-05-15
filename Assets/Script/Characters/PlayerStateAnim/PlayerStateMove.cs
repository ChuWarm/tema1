using Script.Characters;
using UnityEngine;

public class PlayerStateMove : IPlayerState
{
    private PlayerController _playerController;

    public void EnterState(Script.Characters.PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.animator.SetBool(PlayerController.IsWalkingAnim, true);
        Debug.Log("[PlayerStateMove] Entered Move State. IsRun set to true.");
    }

    public void UpdateState()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            _playerController.SetState(PlayerState.Attack);
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playerController.TriggerDash();
            return;
        }
        
        Vector3 input = new(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        float speed = input.magnitude;
        
        if (speed > 0.01f)
        {
            _playerController.HandleMovement();
        }
        else
        {
            if (_playerController.animator.GetBool(PlayerController.IsWalkingAnim))
            {
                 _playerController.animator.SetBool(PlayerController.IsWalkingAnim, false);
                 Debug.Log("[PlayerStateMove] UpdateState: Movement stopped. IsRun set to false.");
            }
            _playerController.SetState(PlayerState.Idle);
        }
        
       
    }

    public void ExitState()
    {
        if (_playerController != null && _playerController.animator != null && _playerController.animator.GetBool(PlayerController.IsWalkingAnim))
        {
            _playerController.animator.SetBool(PlayerController.IsWalkingAnim, false);
            Debug.Log("[PlayerStateMove] ExitState: IsRun set to false.");
        }
        _playerController = null;
    }
}
