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
        var inputVertical = Input.GetAxis("Vertical");
        var inputHorizontal = Input.GetAxis("Horizontal");
        
        // 이동
        if (inputVertical != 0 || inputHorizontal != 0)
        {
            _playerController.SetState(PlayerState.Move);
            return;
        }
        
        // 공격
        if (Input.GetButtonDown("Fire1"))
        {
            _playerController.SetState(PlayerState.Attack);
        }
    }

    public void ExitState()
    {
        _playerController.animator.SetBool(Idle, false);
        _playerController = null;
    }
}
