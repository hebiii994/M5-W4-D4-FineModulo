using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : GuardBaseState
{
    public PatrolState(GuardAI guard) : base(guard) { }

    public override void OnEnter()
    {
        _guard.Agent.speed = _guard.PatrolSpeed;
    }

    public override void OnUpdate()
    {
        if (_guard.IsPlayerInSight())
        {
            _guard.ChangeState(_guard.chaseState);
            return;
        }
       if (!_guard.Agent.pathPending && _guard.Agent.remainingDistance < 0.5f)
        {
            _guard.GoToNextWaypoint();
        }
    }

    public override void OnExit()
    {
        // Extra vediamo dopo
    }
}
