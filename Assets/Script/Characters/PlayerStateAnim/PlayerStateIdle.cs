using Script.Characters;
using UnityEngine;

public class PlayerStateIdle : IPlayerState
{
    private PlayerController _playerController;
    
    public void EnterState(Script.Characters.PlayerController playerController)
    {
        _playerController = playerController;
        if (_playerController.animator.GetBool(PlayerController.IsWalkingAnim))
        {
            _playerController.animator.SetBool(PlayerController.IsWalkingAnim, false);
        }
    }

    public void UpdateState()
    {
        Vector3 input = new(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        float speed = input.magnitude;
        
        // 이동
        if (speed > 0.01)
        {
            _playerController.SetState(PlayerState.Move);
            return;
        }
        
        // 공격
        if (Input.GetButtonDown("Fire1"))
        {
            _playerController.SetState(PlayerState.Attack);
            return;
        }
        
        // 대쉬
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playerController.TriggerDash();
        }
    }

    public void ExitState()
    {
        _playerController = null;
    }
}
