using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : GuardBaseState
{
   
    public AttackState(GuardAI guard) : base(guard) { }
    public override int Priority => 60;
    public override void OnEnter()
    {
        _guard.Agent.velocity = Vector3.zero;
        _guard.Agent.isStopped = true;
        _guard.Agent.updateRotation = false;
        Vector3 directionToPlayer = (_guard.PlayerTransform.position - _guard.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        _guard.transform.rotation = lookRotation;
        _guard.Animator.SetTrigger("Attack");

    }

    public override void OnUpdate()
    {
        _guard.Agent.velocity = Vector3.zero;
    }

    public override void OnExit()
    {
        _guard.Agent.isStopped = false;
        _guard.Agent.updateRotation = true;
    }
}

