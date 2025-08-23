using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : GuardBaseState
{
    private const float REACTION_TIME_BEFORE_SEARCHING = 0.5f;
    public override int Priority => 50;
    public ChaseState(GuardAI guard) : base(guard) { }
    public override void OnEnter()
    {
        //_timeLostPlayer = -1f;
        //_guard.Agent.enabled = true;
        if (!_guard.Agent.enabled)
            _guard.Agent.enabled = true;

        _guard.Agent.isStopped = false;
        _guard.Agent.updateRotation = true;
        _guard.Agent.speed = _guard.ChaseSpeed;
        AlertManager.RegisterChaser(_guard);
        if (!AlertManager.IsAlertActive)
        {
            AlertManager.TriggerAlert();
            _guard.BroadcastAlert(); 
        }
    }
    public override void OnUpdate()
    {
        if (Time.time < _guard.LastHitTime + _guard.StunDuration)
        {
            _guard.Agent.velocity = Vector3.zero;
            return;
        }

        if (!_guard.IsPlayerInSight())
        {
            _guard.LastKnownPlayerPosition = _guard.PlayerTransform.position;
            _guard.ChangeState(_guard.alertState, true);
            return;
        }

        float distanceToPlayer = Vector3.Distance(_guard.transform.position, _guard.PlayerTransform.position);
        if (distanceToPlayer < _guard.CatchDistance)
        {
            if (Time.time - _guard.LastDamageTime > _guard.AttackRate)
            {
                _guard.ChangeState(_guard.attackState);
                return;
            }
        }

        AlertManager.ReportPlayerSeen();
        _guard.Agent.SetDestination(_guard.PlayerTransform.position);
    }
    public override void OnExit()
    {
         _guard.Agent.speed = _guard.PatrolSpeed;
        AlertManager.UnregisterChaser(_guard);
    }
}
