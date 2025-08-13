using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingState : GuardBaseState
{
    private bool _hasSetDestination;

    public SearchingState(GuardAI guard) : base(guard) { }
    public override void OnEnter()
    {
        
        Debug.Log("Guardia: Entro in stato Searching (movimento).");
        _guard.Agent.enabled = true;
        _guard.Agent.updateRotation = true;
        _guard.Agent.speed = _guard.PatrolSpeed;
        _hasSetDestination = false;
    }
    
    public override void OnUpdate()
    {
        if (!_hasSetDestination && _guard.Agent.isOnNavMesh)
        {
            _guard.Agent.SetDestination(_guard.LastKnownPlayerPosition);
            _hasSetDestination = true;
        }

        if (_guard.IsPlayerInSight())
        {
            _guard.ChangeState(_guard.chaseState);
            return;
        }

        if (!_guard.Agent.pathPending && _guard.Agent.remainingDistance <= _guard.Agent.stoppingDistance)
        {

            if (!_guard.Agent.hasPath || _guard.Agent.velocity.sqrMagnitude == 0f)
            {
                _guard.ChangeState(_guard.lookAroundState);
            }
        }
    }

    public override void OnExit()
    {

        Debug.Log("Exiting Searching State");

    }
}
