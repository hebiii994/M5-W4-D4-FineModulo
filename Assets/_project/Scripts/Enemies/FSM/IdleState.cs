using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : GuardBaseState
{
    private float _waitTimer;
    private Quaternion _targetRotation;


    public IdleState(GuardAI guard) : base(guard) { }

    public override void OnEnter()
    {
        _guard.Agent.enabled = true;
        _guard.Agent.updateRotation = true;
        _guard.Agent.SetDestination(_guard.StartingPosition);
        _guard.Agent.speed = _guard.PatrolSpeed;
        _waitTimer = _guard.IdleWaitTime;
        _targetRotation = _guard.StartingRotation;
    }

    public override void OnUpdate()
    {
        if (_guard.IsPlayerInSight())
        {
            _guard.ChangeState(_guard.chaseState);
            return;
        }

        if (!_guard.Agent.pathPending && _guard.Agent.remainingDistance <= _guard.Agent.stoppingDistance)
        {

            _guard.Agent.updateRotation = false;

            _guard.transform.rotation = Quaternion.RotateTowards( _guard.transform.rotation,_targetRotation,_guard.Agent.angularSpeed * Time.deltaTime);

            if (Quaternion.Angle(_guard.transform.rotation, _targetRotation) < 1f)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0)
                {
                    float initialYAngle = _guard.StartingRotation.eulerAngles.y;
                    float randomOffset = Random.Range(-90f, 90f);
                    _targetRotation = Quaternion.Euler(0, initialYAngle + randomOffset, 0);
                    _waitTimer = _guard.IdleWaitTime;
                }
            }
        }
        else
        {
            _guard.Agent.updateRotation = true;
        }
    }

    public override void OnExit()
    {
        _guard.Agent.updateRotation = true;
    }

}  
