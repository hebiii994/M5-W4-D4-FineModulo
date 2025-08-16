using UnityEngine;

public class HurtState : PlayerBaseState
{
    public HurtState(PlayerController controller) : base(controller) { }

    public override void OnEnter()
    {
        Debug.Log("Entering Hurt State");
        _controller.StopMovement();

        _controller.Animator.SetBool("isCrouching", false);
        _controller.Animator.SetBool("isCrawling", false);
        _controller.CapsuleCollider.height = _controller.NormalHeight;
        _controller.CapsuleCollider.center = new Vector3(0, _controller.NormalHeight / 2f, 0);

        _controller.Animator.speed = 1f;
        _controller.Animator.SetTrigger("GetHit");
    }

    public override void OnUpdate() { }

    public override void OnExit() { }
}