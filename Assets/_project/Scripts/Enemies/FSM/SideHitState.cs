using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideHitState : GuardBaseState
{
    public override int Priority => 80;
    public SideHitState(GuardAI guard) : base(guard) { }

    public override void OnEnter()
    {
        _guard.Agent.velocity = Vector3.zero;
        _guard.Agent.isStopped = true;
        //_guard.Agent.enabled = false;
    }

    public override void OnUpdate()
    {
    }

    public override void OnExit()
    {
        _guard.Agent.isStopped = false;
    }
}
