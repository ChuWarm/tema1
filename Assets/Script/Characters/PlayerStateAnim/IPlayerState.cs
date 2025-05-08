using Script.Characters;
using UnityEngine;

public interface IPlayerState
{
    void EnterState(Script.Characters.PlayerController playerController);
    void UpdateState();
    void ExitState();
}
