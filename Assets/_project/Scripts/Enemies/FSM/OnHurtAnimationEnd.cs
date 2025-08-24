using System.Collections;
using UnityEngine;

public class OnHurtAnimationEnd : StateMachineBehaviour
{
    private GuardAI guard;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (guard == null)
        {
            guard = animator.GetComponentInParent<GuardAI>();
        }
        animator.SetInteger("HitType", 0);


        Debug.Log("OnStateEnter: Entrato in uno stato di danno. Lo script OnHurtAnimationEnd è attivo!");
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GuardAI guard = animator.GetComponentInParent<GuardAI>();
        if (guard == null || guard.IsDead) return;

        if (!guard.Agent.enabled) guard.Agent.enabled = true;
        guard.Agent.isStopped = false;

        if (stateInfo.IsName("Fall"))
        {
            guard.ChangeState(guard.getUpState, true);
        }
        else
        {
            if (guard.IsPlayerInSight())
            {
                Debug.Log("OnStateExit: Giocatore in vista dopo il colpo. Cambio stato a Chase.");
                guard.ChangeState(guard.chaseState, true);
            }
            else
            {
                guard.StartCoroutine(DelayedSearch(1f));
            }


        }
    }
    private IEnumerator DelayedSearch(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (guard != null && !guard.IsDead)
        {
            if (guard.IsPlayerInSight())
            {
                guard.ChangeState(guard.chaseState, true);
            }
            else
            {
                Debug.Log("OnHurtAnimationEnd: Nessun player in vista → entro in SearchingState.");
                guard.ChangeState(guard.searchingState, true);
            }
        }
    }
}

