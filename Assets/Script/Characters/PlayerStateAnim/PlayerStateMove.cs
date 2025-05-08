using UnityEngine;

public class PlayerStateMove : IPlayerState
{
    private static readonly int IsRun = Animator.StringToHash("IsRun");
    private PlayerController _playerController;

    public void EnterState(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.currentLookMode = LookMode.Movement;
        _playerController.Animator.SetBool(IsRun, true);
    }

    public void UpdateState()
    {
        Vector3 input = new(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        float speed = input.magnitude;
        
        if (speed > 0.1f)
        {
            _playerController.Move(input);
        }
        else
        {
            AnimatorStateInfo currentAnim = _playerController.Animator.GetCurrentAnimatorStateInfo(0);
            if (currentAnim.IsName("RunStart"))
            {
                _playerController.Animator.Play(PlayerStateIdle.Idle);
            }
            
            _playerController.SetState(PlayerState.Idle);
            return;
        }
        
        if (Input.GetButtonDown("Fire1"))
        {
            _playerController.SetState(PlayerState.Attack);
        }
    }

    public void ExitState()
    {
        _playerController.Animator.SetBool(IsRun, false);
        _playerController = null;
    }
}
