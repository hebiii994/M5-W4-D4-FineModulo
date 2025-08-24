using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlertState : GuardBaseState
{
    public override int Priority => 40;

    public AlertState(GuardAI guard) : base(guard) { }
    public override void OnEnter()
    {
        if (!_guard.Agent.enabled)
            _guard.Agent.enabled = true;
        Debug.Log($"--- ENTER ALERT STATE --- Destinazione: {_guard.LastKnownPlayerPosition}, Posizione Agente: {_guard.Agent.nextPosition}, Posizione Transform: {_guard.transform.position}, Agente Attivo: {_guard.Agent.enabled}, Agente Stoppato: {_guard.Agent.isStopped}");
        _guard.Agent.isStopped = false;
        _guard.Agent.speed = _guard.ChaseSpeed;
        _guard.Agent.updateRotation = false;
        _guard.Agent.SetDestination(_guard.LastKnownPlayerPosition);
    }

    public override void OnUpdate()
    {
        if (!AlertManager.IsAlertActive)
        {
            _guard.ReturnToDefaultState();
            return;
        }
        if (_guard.IsPlayerInSight())
        {
            Debug.Log("AlertState: Player avvistato da vicino → passo a Chase.");
            _guard.ChangeState(_guard.chaseState);
            return;
        }
        Vector3 direction = (_guard.LastKnownPlayerPosition - _guard.transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _guard.transform.rotation = Quaternion.RotateTowards(
                _guard.transform.rotation,
                targetRotation,
                _guard.Agent.angularSpeed * Time.deltaTime
            );
        }


        if (_guard.Agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            _guard.ChangeState(_guard.searchingState);
            return;
        }

        if (!AlertManager.IsAlertActive)
        {
            _guard.ChangeState(_guard.searchingState);
            return;
        }


        

        if (!_guard.Agent.pathPending && _guard.Agent.remainingDistance <= _guard.Agent.stoppingDistance)
        {
            _guard.ChangeState(_guard.searchingState, true);
        }
    }

    public override void OnExit()
    {
        if (_guard.Agent.isOnNavMesh)
        {
            _guard.Agent.updateRotation = true;
        }
        _guard.Agent.isStopped = false;
    }

}
