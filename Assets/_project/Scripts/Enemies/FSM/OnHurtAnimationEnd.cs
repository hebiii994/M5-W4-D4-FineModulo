using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHurtAnimationEnd : StateMachineBehaviour
{
    [SerializeField] private bool _isEndOfSequence = false;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("HitType", 0);
        Debug.Log("OnStateEnter: Entrato in uno stato di danno. Lo script OnHurtAnimationEnd è attivo!");
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("OnStateExit: Uscendo dallo stato di danno. Cambio stato FSM a Searching...");


        if (_isEndOfSequence)
        {
            GuardAI guard = animator.GetComponentInParent<GuardAI>();
            if (guard != null)
            {
                guard.ChangeState(guard.searchingState);
            }
        }

    }
}

