using UnityEngine;

public class OnAttackStateExit : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GuardAI guard = animator.GetComponentInParent<GuardAI>();
        if (guard != null)
        {
            guard.ChangeState(guard.chaseState, true);
        }
    }
}