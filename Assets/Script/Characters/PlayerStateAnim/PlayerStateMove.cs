using Script.Characters;
using UnityEngine;

public class PlayerStateMove : IPlayerState
{
    private PlayerController _playerController;

    public void EnterState(Script.Characters.PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.animator.SetBool(PlayerController.IsWalkingAnim, true);
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
        
        if (speed > 0.01 && _playerController.IsRun)
        {
            _playerController.HandleMovement();
        }
        else
        {
            AnimatorStateInfo currentAnim = _playerController.animator.GetCurrentAnimatorStateInfo(0);
            if (currentAnim.IsName("RunStart"))
            {
                _playerController.animator.Play(PlayerController.Idle);
            }
            _playerController.SetState(PlayerState.Idle);
        }
        
       
    }

    public void ExitState()
    {
        _playerController.animator.SetBool(PlayerController.IsWalkingAnim, false);
        _playerController = null;
    }
}
