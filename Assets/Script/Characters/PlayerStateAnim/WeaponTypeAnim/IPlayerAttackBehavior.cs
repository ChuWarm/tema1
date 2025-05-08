using Script.Characters;

public interface IPlayerAttackBehavior
{
    void Enter(PlayerController player);
    void Update();
    void Exit();
}