using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLookAroundEnd : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        GuardAI guard = animator.GetComponentInParent<GuardAI>();
        if (guard != null)
        {
            guard.OnLookAroundFinished();
        }
    }
}
