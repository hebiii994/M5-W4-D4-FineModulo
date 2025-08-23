using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OnHurtAnimationEnd : StateMachineBehaviour
{
    private GuardAI guard;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("HitType", 0);
        Debug.Log("OnStateEnter: Entrato in uno stato di danno. Lo script OnHurtAnimationEnd è attivo!");
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GuardAI guard = animator.GetComponentInParent<GuardAI>();
        if (guard == null || guard.IsDead)
        {
            return;
        }

        if (stateInfo.IsName("Fall"))
        {
            guard.ChangeState(guard.getUpState, true);
        }
        else 
        {
            if (guard.IsPlayerInSight())
            {
                guard.ChangeState(guard.chaseState, true);
            }
            else
            {
                guard.ChangeState(guard.alertState);
            }
        }

    }
    
}

