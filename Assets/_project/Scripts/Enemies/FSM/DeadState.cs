using UnityEngine;
using UnityEngine.AI;

public class DeadState : GuardBaseState
{
    public DeadState(GuardAI guard) : base(guard) { }

    public override void OnEnter()
    {
        if (_guard.Agent.isActiveAndEnabled)
        {
            _guard.Agent.velocity = Vector3.zero;
            _guard.Agent.isStopped = true;
            _guard.Agent.enabled = false;
        }
            Debug.Log("Guardia morta!");
    }


    public override void OnUpdate() { }

    public override void OnExit() { }
}