using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : GuardBaseState
{
    private float _timeLostPlayer = -1f;
    private const float REACTION_TIME_BEFORE_SEARCHING = 0.5f;
    public ChaseState(GuardAI guard) : base(guard) { }
    public override void OnEnter()
    {
        _timeLostPlayer = -1f;
        _guard.Agent.enabled = true;
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

        if (_guard.Animator.GetCurrentAnimatorStateInfo(0).IsTag("Hurt") || _guard.Animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            return;
        }
        if (_guard.PlayerController.CurrentState is PlayerAttackState)
        {
            // Insegui ma non fare danno, aspetta che finisca la sua combo
        }
        else 
        {
            float distanceToPlayer = Vector3.Distance(_guard.transform.position, _guard.PlayerTransform.position);
            if (Time.time > _guard.LastHitTime + _guard.StunDuration)
            {
                if (distanceToPlayer < _guard.CatchDistance && Time.time - _guard.LastDamageTime > _guard.AttackRate)
                {
                    _guard.ChangeState(_guard.attackState);
                    return;
                }
            }     
            if (!_guard.IsPlayerInSight())
            {
                _guard.LastKnownPlayerPosition = _guard.PlayerTransform.position;
                _guard.ChangeState(_guard.alertState);
            }
        }
        if (!_guard.IsPlayerInSight())
        {
            if (_timeLostPlayer < 0)
            {
                _timeLostPlayer = Time.time;
            }
            if (Time.time >= _timeLostPlayer + REACTION_TIME_BEFORE_SEARCHING)
            {
                _guard.LastKnownPlayerPosition = _guard.PlayerTransform.position;
                _guard.ChangeState(_guard.searchingState);
            }
        }
        else
        {
            _timeLostPlayer = -1f;
        }
        AlertManager.ReportPlayerSeen();

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
        AlertManager.UnregisterChaser(_guard);
    }
}
