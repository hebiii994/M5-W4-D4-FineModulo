using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlertState : GuardBaseState
{
    public override int Priority => 40;
    public AlertState(GuardAI guard) : base(guard) { }
    public override void OnEnter()
    {
        if (!_guard.Agent.enabled)
            _guard.Agent.enabled = true;

        _guard.Agent.isStopped = false;
        _guard.Agent.speed = _guard.ChaseSpeed; 
        _guard.Agent.SetDestination(_guard.LastKnownPlayerPosition);
    }

    public override void OnUpdate()
    {
        if (!AlertManager.IsAlertActive)
        {
            _guard.ReturnToDefaultState();
            return;
        }

        if (_guard.Agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            _guard.ChangeState(_guard.searchingState);
            return;
        }

        if (!AlertManager.IsAlertActive)
        {
            _guard.ChangeState(_guard.searchingState);
            return;
        }


        if (_guard.IsPlayerInSight())
        {
            _guard.ChangeState(_guard.chaseState);
        }

        if (!_guard.Agent.pathPending && _guard.Agent.remainingDistance <= _guard.Agent.stoppingDistance)
        {
            _guard.ChangeState(_guard.searchingState, true);
        }
    }

    public override void OnExit()
    {
        _guard.Agent.isStopped = false;
    }
}
