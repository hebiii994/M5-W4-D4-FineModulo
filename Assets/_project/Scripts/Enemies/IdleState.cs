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
        _guard.Agent.updateRotation = false;
        _guard.Agent.SetDestination(_guard.StartingPosition);
        _targetRotation = _guard.StartingRotation;
        _waitTimer = _guard.IdleWaitTime;
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
            _guard.transform.rotation = Quaternion.RotateTowards(_guard.transform.rotation,_targetRotation,_guard.Agent.angularSpeed * Time.deltaTime);
            if (Quaternion.Angle(_guard.transform.rotation, _targetRotation) < 1f)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0)
                {

                    float randomYAngle = Random.Range(0f, 360f);
                    _targetRotation = Quaternion.Euler(0, randomYAngle, 0);
                    _waitTimer = _guard.IdleWaitTime;
                }
            }
            
        }


    }

    public override void OnExit()
    {
        _guard.Agent.updateRotation = true;
    }

}  
