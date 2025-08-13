using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideHitState : GuardBaseState
{
    public SideHitState(GuardAI guard) : base(guard) { }

    public override void OnEnter()
    {

        _guard.Agent.enabled = false;
    }

    public override void OnUpdate()
    {
    }

    public override void OnExit()
    {
        _guard.Agent.enabled = true;
    }
}
