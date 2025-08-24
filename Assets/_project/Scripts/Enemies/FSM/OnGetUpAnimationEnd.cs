using UnityEngine;
public class OnGetUpAnimationEnd : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GuardAI guard = animator.GetComponentInParent<GuardAI>();
        if (guard == null || guard.IsDead) return;
        Debug.Log($"--- ENTER GETUP ANIMATION --- Posizione Agente: {guard.Agent.nextPosition}, Posizione Transform: {guard.transform.position}.");
        guard.ChangeState(guard.getUpState, true);
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GuardAI guard = animator.GetComponentInParent<GuardAI>();
        if (guard == null || guard.IsDead) return;
        guard.Agent.Warp(guard.transform.position);
        guard.Agent.isStopped = false;

        if (guard.IsPlayerInSight())
        {
            guard.LastKnownPlayerPosition = guard.PlayerTransform.position;
            guard.ChangeState(guard.chaseState, true);
        }
        else if (AlertManager.IsAlertActive)
        {
            if (AlertManager.SharedLastKnownPlayerPosition != Vector3.zero)
            {
                guard.LastKnownPlayerPosition = AlertManager.SharedLastKnownPlayerPosition;
            }
            guard.ChangeState(guard.alertState, true);
        }
        else
        {
            guard.ReturnToDefaultState();
        }
    }
}