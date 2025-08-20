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
        Transform minimapIcon = _guard.transform.Find("MinimapIcon");
        if (minimapIcon != null)
        {
            // Se lo trova, disattiva il suo GameObject
            minimapIcon.gameObject.SetActive(false);
        }
        Debug.Log("Guardia morta!");
    }


    public override void OnUpdate() { }

    public override void OnExit() { }
}