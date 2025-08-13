using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : GuardBaseState
{
    public FallState(GuardAI guard) : base(guard) { }

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
