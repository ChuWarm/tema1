using Script.Characters;
using UnityEngine;

public class PlayerStateDash : IPlayerState
{
    private PlayerController _playerController;
    private float _dashDuration = 0.25f;
    private float _dashSpeed = 50f;
    private float _elapsedTime;
    private Vector3 _dashDirection;
    
    public void EnterState(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.animator.ResetTrigger("Attack");
        _playerController.animator.CrossFade(PlayerController.DashAnim, 0.05f);

        _playerController.IsRun = true;
        _playerController.IsAttacking = false;
        
        _dashDirection = _playerController.transform.forward;
        _elapsedTime = 0f;
    }

    public void UpdateState()
    {
        _elapsedTime += Time.deltaTime;

        // 대쉬
        if (_elapsedTime < _dashDuration)
        {
            _playerController.characterController.Move(_dashDirection * (_dashSpeed * Time.deltaTime));
        }
        else if (_elapsedTime >= _dashDuration && 
                 _playerController.animator.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
        {
            Vector3 input = new(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            
            _playerController.SetState(input.magnitude > 0.1f ? PlayerState.Move : PlayerState.Idle);
        }
    }

    public void ExitState()
    {
        _playerController = null;
    }
}
