using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : GuardBaseState
{
    private float _attackTimer;
    public AttackState(GuardAI guard) : base(guard) { }

    public override void OnEnter()
    {
        _guard.Agent.isStopped = true;
        _guard.Agent.updateRotation = false;
        Vector3 directionToPlayer = (_guard.PlayerTransform.position - _guard.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        _guard.transform.rotation = lookRotation;
        _guard.Animator.SetTrigger("Attack");
        _attackTimer = _guard.AttackRate;
    }

    public override void OnUpdate()
    {
        _attackTimer -= Time.deltaTime;


        if (_attackTimer <= 0f)
        {
            _guard.ChangeState(_guard.chaseState);
        }
    }

    public override void OnExit()
    {
        _guard.Agent.isStopped = false;
        _guard.Agent.updateRotation = true;
    }
}
