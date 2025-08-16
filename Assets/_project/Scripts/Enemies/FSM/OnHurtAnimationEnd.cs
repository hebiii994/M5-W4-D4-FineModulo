using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHurtAnimationEnd : StateMachineBehaviour
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("HitType", 0);
        Debug.Log("OnStateEnter: Entrato in uno stato di danno. Lo script OnHurtAnimationEnd è attivo!");
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GuardAI guard = animator.GetComponentInParent<GuardAI>();
        if (guard == null) return;
        if (stateInfo.IsName("Fall"))
        {
            Debug.Log("OnStateExit: Uscito dallo stato di caduta. Cambio stato a GetUp.");
            guard.ChangeState(guard.getUpState);
        }
        else
        {

            if (guard.IsPlayerInSight())
            {

                Debug.Log("OnStateExit: Giocatore in vista dopo il colpo. Cambio stato a Chase.");
                guard.ChangeState(guard.chaseState);
            }
            else
            {
                Debug.Log("OnStateExit: Giocatore non in vista dopo il colpo. Cambio stato a Searching.");
                guard.ChangeState(guard.searchingState);
            }
        }

    }
}

