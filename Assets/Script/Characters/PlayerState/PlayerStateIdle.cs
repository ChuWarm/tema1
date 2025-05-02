using UnityEngine;

public class PlayerStateIdle : IPlayerState
{
    private PlayerController _playerController;
    
    public void EnterState(PlayerController playerController)
    {
        _playerController = playerController;
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
        _playerController = null;
    }
}
