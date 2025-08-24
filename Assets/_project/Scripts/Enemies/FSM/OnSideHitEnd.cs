using UnityEngine;

public class OnSideHitEnd : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("HitType", 0);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GuardAI guard = animator.GetComponentInParent<GuardAI>();
        if (guard == null || guard.IsDead) return;


        if (guard.IsPlayerInSight())
        {
            guard.ChangeState(guard.chaseState, true);
        }
        else
        {
            guard.ChangeState(guard.alertState, true);
        }
    }
}