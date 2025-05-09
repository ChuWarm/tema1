using System.Collections;
using System.Collections.Generic;
using Script.Characters;
using UnityEngine;

public class PlayerAnimatorStateAttack : StateMachineBehaviour
{
    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<PlayerController>().SetState(PlayerState.Idle);
    }
}
