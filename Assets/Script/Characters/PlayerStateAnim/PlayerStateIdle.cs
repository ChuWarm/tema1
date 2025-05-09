using Script.Characters;
using UnityEngine;

public class PlayerStateIdle : IPlayerState
{
    public static readonly int Idle = Animator.StringToHash("Idle");
    private PlayerController _playerController;
    
    public void EnterState(Script.Characters.PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.animator.SetBool(Idle, true);
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
    }

    public void ExitState()
    {
        _playerController.animator.SetBool(Idle, false);
        _playerController = null;
    }
}
