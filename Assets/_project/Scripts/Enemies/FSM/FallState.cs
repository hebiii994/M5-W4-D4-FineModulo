using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : GuardBaseState
{
    public FallState(GuardAI guard) : base(guard) { }
    public override int Priority => 90;
    public override void OnEnter()
    {
        _guard.Agent.velocity = Vector3.zero;
        _guard.Agent.isStopped = true;
        Debug.Log($"--- ENTER FALL STATE --- Posizione Agente: {_guard.Agent.nextPosition}, Posizione Transform: {_guard.transform.position}, Agente Attivo: {_guard.Agent.enabled}, Agente Stoppato: {_guard.Agent.isStopped}");
        //_guard.Agent.enabled = false;
    }

    public override void OnUpdate()
    {
    }

    public override void OnExit()
    {
        _guard.Agent.isStopped = false;
    }
}
