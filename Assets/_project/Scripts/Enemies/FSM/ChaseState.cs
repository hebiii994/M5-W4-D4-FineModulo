using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : GuardBaseState
{
    public ChaseState(GuardAI guard) : base(guard) { }
    public override void OnEnter()
    {
        _guard.Agent.updateRotation = true;
        _guard.Agent.speed = _guard.ChaseSpeed;
        AlertManager.TriggerAlert();
        _guard.BroadcastAlert();
    }
    public override void OnUpdate()
    {
        if (_guard.Animator.GetCurrentAnimatorStateInfo(0).IsTag("Hurt"))
        {
            return;
        }
        if (_guard.PlayerTransform.GetComponent<PlayerCombat>().IsAttacking)
        {
            // Insegui ma non fare danno, aspetta che finisca la sua combo
        }
        else 
        {
            float distanceToPlayer = Vector3.Distance(_guard.transform.position, _guard.PlayerTransform.position);

            if (distanceToPlayer < _guard.CatchDistance && Time.time - _guard.LastDamageTime > _guard.AttackRate)
            {

                if (_guard.PlayerTransform.TryGetComponent(out PlayerHealth playerHealth))
                {

                    playerHealth.TakeDamage((int)_guard.DamageAmount);
                    _guard.UpdateLastDamageTime();
                }
                return;
            }
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
    }
}
