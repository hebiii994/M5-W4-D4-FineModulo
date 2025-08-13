using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : GuardBaseState
{
    public PatrolState(GuardAI guard) : base(guard) { }

    public override void OnEnter()
    {
        _guard.Agent.enabled = true;
        _guard.Agent.updateRotation = true;
        _guard.Agent.speed = _guard.PatrolSpeed;

        if (_guard.Waypoints.Length > 0)
        {
            float closestDistance = float.MaxValue;
            int closestWaypointIndex = 0;
            for (int i = 0; i < _guard.Waypoints.Length; i++)
            {
                float distance = Vector3.Distance(_guard.transform.position, _guard.Waypoints[i].position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestWaypointIndex = i;
                }
            }
            _guard.SetCurrentWaypointIndex(closestWaypointIndex);
            _guard.GoToNextWaypoint();
        }
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
        _guard.Agent.updateRotation = true;
    }
}
