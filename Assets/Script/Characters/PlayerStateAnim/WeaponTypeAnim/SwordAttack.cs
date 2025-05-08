using UnityEngine;

public class SwordAttack : IPlayerAttackBehavior
{
    private PlayerController _playerController;
    
    public void Enter(PlayerController player)
    {
        _playerController = player;
        _playerController.currentLookMode = LookMode.Mouse;
        _playerController.LookAtMouse();
        _playerController.TirggerAttack();
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            _playerController.LookAtMouse();
            _playerController.TirggerAttack();
        }
    }

    public void Exit()
    {
        _playerController = null;
    }
}
