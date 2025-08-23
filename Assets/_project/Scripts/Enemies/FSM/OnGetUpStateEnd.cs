using UnityEngine;
using UnityEngine.AI;

public class OnGetUpStateEnd : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GuardAI guard = animator.GetComponentInParent<GuardAI>();
        if (guard != null)
        {
            if (guard.Agent.isOnNavMesh)
            {
                guard.Agent.isStopped = false;
            }

            guard.ChangeState(guard.chaseState, true);
        }
    }
}