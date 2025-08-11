using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingState : GuardBaseState
{
    private float _searchTimer;

    public SearchingState(GuardAI guard) : base(guard) { }
    public override void OnEnter()
    {
        _guard.Agent.SetDestination(_guard.LastKnownPlayerPosition);
        _guard.Agent.speed = _guard.PatrolSpeed;
        _searchTimer = _guard.SearchTime;
    }
    
    public override void OnUpdate()
    {
        if (!_guard.Agent.pathPending && _guard.Agent.remainingDistance < 0.5f)
        {
            _searchTimer -= Time.deltaTime;
            if (_searchTimer <= 0f)
            {
                if (_guard.CurrentBehaviorType == GuardAI.BehaviorType.Stationary)
                {
                    _guard.ChangeState(_guard.idleState);
                    Debug.Log("Search time completed, returning to idle.");
                }
                else
                {
                    _guard.ChangeState(_guard.patrolState);
                    Debug.Log("Search time completed, returning to patrol.");
                }
                    

            }
        }
        
        if (_guard.IsPlayerInSight())
        {
            Debug.Log("Player sighted during search, switching to chase state.");
            _guard.ChangeState(_guard.chaseState);
        }
    }

    public override void OnExit()
    {
        Debug.Log("Exiting Searching State");

    }
}
