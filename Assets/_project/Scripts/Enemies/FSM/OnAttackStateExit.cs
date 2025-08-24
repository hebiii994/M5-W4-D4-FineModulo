using UnityEngine;

public class OnAttackStateExit : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GuardAI guard = animator.GetComponentInParent<GuardAI>();
        if (guard != null)
        {
            if (guard != null)
            {
                if (guard.IsDead)
                {
                    return;
                }

                if (guard.CurrentState is FallState || guard.CurrentState is SideHitState)
                {
                    return; 
                }
                guard.ChangeState(guard.chaseState, true);
            }
        }
    }
}