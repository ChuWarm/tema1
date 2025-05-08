using Script.Characters;
using UnityEngine;

public class PlayerStateMove : IPlayerState
{
    private static readonly int IsRun = Animator.StringToHash("IsRun");
    private PlayerController _playerController;

    public void EnterState(Script.Characters.PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.animator.SetBool(IsRun, true);
        _playerController.currentLookMode = LookMode.Movement;
    }

    public void UpdateState()
    {
        Vector3 input = new(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        float speed = input.magnitude;
        
        if (speed == 0)
        {
            AnimatorStateInfo currentAnim = _playerController.animator.GetCurrentAnimatorStateInfo(0);
            if (currentAnim.IsName("RunStart"))
            {
                _playerController.animator.Play(PlayerStateIdle.Idle);
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
        _playerController.animator.SetBool(IsRun, false);
        _playerController = null;
    }
}
