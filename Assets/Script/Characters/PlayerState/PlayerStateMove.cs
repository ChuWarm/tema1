using UnityEngine;

public class PlayerStateMove : IPlayerState
{
    private const float _gravity = -9.81f;
    
    private PlayerController _playerController;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _movePosition;
    private float _moveSpeed;

    public void EnterState(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public void UpdateState()
    {
        Move();
    }

    public void ExitState()
    {
        _playerController = null;
    }
    
    private void Move()
    {
        var inputVertical = Input.GetAxis("Vertical");
        var inputHorizontal = Input.GetAxis("Horizontal");
        
        Vector3 inputDirection = new Vector3(inputHorizontal, 0f, inputVertical).normalized;
        Vector3 moveDir = Vector3.zero;
        
        _velocity.y += _gravity * Time.deltaTime;
        _movePosition.y = _velocity.y * Time.deltaTime;
        
        if (inputVertical != 0 || inputHorizontal != 0)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();
            
            moveDir = cameraForward * inputDirection.z + cameraRight * inputDirection.x;
            
            _playerController.transform.rotation = Quaternion.LookRotation(moveDir);
            
            _moveSpeed = Input.GetKey(KeyCode.LeftShift) ? 20f : 10f;
            
            Vector3 finalMove = moveDir * _moveSpeed + _velocity;
            _playerController.characterController.Move(finalMove * Time.deltaTime);
        }
        else
        {
            _playerController.SetState(PlayerState.Idle);
        }
    }
}
