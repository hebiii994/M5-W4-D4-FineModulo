using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAroundState : GuardBaseState
{
    public LookAroundState(GuardAI guard) : base(guard) { }

    public override void OnEnter()
    {
        _guard.Agent.isStopped = true;
        _guard.Agent.updateRotation = false;
        _guard.Animator.SetTrigger("LookAround");
    }

    public override void OnUpdate()
    {

        if (_guard.IsPlayerInSight())
        {
            _guard.ChangeState(_guard.chaseState);
            return;
        }
    }

    public override void OnExit()
    {
        _guard.Agent.isStopped = false;
        _guard.Agent.updateRotation = true;
    }
}
