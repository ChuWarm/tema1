using UnityEngine;

public class GunAttack : IPlayerAttackBehavior
{
    private PlayerController _playerController;
    private float moveSpeedDuringAttack = 5f;
    
    public void Enter(PlayerController player)
    {
        _playerController = player;
        _playerController.currentLookMode = LookMode.Mouse;
        _playerController.TirggerAttack();

    }

    public void Update()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _playerController.Move(input, moveSpeedDuringAttack);
    }

    public void Exit()
    {
        _playerController = null;
    }
}
