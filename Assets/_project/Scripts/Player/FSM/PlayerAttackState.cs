using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerController controller) : base(controller) { }

    public override void OnEnter()
    {
        Debug.Log("Entering Attack State");
        _controller.StopMovement();
        _controller.CanMove = false;
        Attack();
    }

    public override void OnUpdate() { }

    public override void OnExit()
    {
        Debug.Log("Exiting Attack State");
        _controller.CanMove = true;
        _controller.Animator.ResetTrigger("Attack");
    }

    private void Attack()
    {
        if (Time.time - _controller.Combat.LastAttackTime > _controller.Combat.ComboResetTime)
        {
            _controller.Combat.ComboStep = 1;
        }
        else
        {
            _controller.Combat.ComboStep++;
            if (_controller.Combat.ComboStep > 3) _controller.Combat.ComboStep = 1;
        }

        _controller.Combat.LastAttackTime = Time.time;

        _controller.Animator.SetInteger("AttackCombo", _controller.Combat.ComboStep);
        _controller.Animator.SetTrigger("Attack");
    }


    public void OnAnimationFinished()
    {
        _controller.ChangeState(_controller.locomotionState);
    }
}