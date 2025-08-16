using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : PlayerBaseState
{
    public CrouchState(PlayerController controller) : base(controller) { }

    public override void OnEnter()
    {
        Debug.Log("Entering Crouch State (fermo)");
        _controller.StopMovement();
        _controller.Animator.SetBool("isCrouching", true);
        _controller.CapsuleCollider.height = _controller.CrouchHeight;
        _controller.CapsuleCollider.center = new Vector3(0, _controller.CrouchHeight / 2f, 0);
    }

    public override void OnUpdate()
    {

        if (_controller.CrouchInputDown)
        {
            _controller.ChangeState(_controller.locomotionState);
            return;
        }

        if (_controller.MovementInput.sqrMagnitude > 0.01f)
        {
            _controller.ChangeState(_controller.crawlState);
            return;
        }
    }

    public override void OnExit() { }
}
