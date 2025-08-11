using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : GuardBaseState
{
    public ChaseState(GuardAI guard) : base(guard) { }
    public override void OnEnter()
    {
        _guard.Agent.speed = _guard.ChaseSpeed;
        
    }
    public override void OnUpdate()
    {
        float distanceToPlayer = Vector3.Distance(_guard.transform.position, _guard.PlayerTransform.position);

        if (distanceToPlayer < _guard.CatchDistance)
        {
            _guard.CatchPlayer(); //in futuro magari dmg vediamo
            return; 
        }

        _guard.Agent.SetDestination(_guard.PlayerTransform.position);
        if (!_guard.IsPlayerInSight())
        {
            _guard.LastKnownPlayerPosition = _guard.PlayerTransform.position;
            _guard.ChangeState(_guard.searchingState);
        }

    }
    public override void OnExit()
    {
         _guard.Agent.speed = _guard.PatrolSpeed;

    }
}
