using Script.Characters;
using UnityEngine;

public interface IPlayerState
{
    void EnterState(PlayerController playerController);
    void UpdateState();
    void ExitState();
}
