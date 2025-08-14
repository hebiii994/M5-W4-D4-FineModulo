using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : GuardBaseState
{
    public AlertState(GuardAI guard) : base(guard) { }
    public override void OnEnter()
    {
        _guard.Agent.speed = _guard.ChaseSpeed; 
        _guard.Agent.SetDestination(_guard.LastKnownPlayerPosition);
    }

    public override void OnUpdate()
    {

        if (!AlertManager.IsAlertActive)
        {
            _guard.ChangeState(_guard.searchingState);
            return;
        }


        if (_guard.IsPlayerInSight())
        {
            _guard.ChangeState(_guard.chaseState);
        }
    }

    public override void OnExit() { }
}
