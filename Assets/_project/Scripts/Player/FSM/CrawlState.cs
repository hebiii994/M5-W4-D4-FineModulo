using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlState : PlayerBaseState
{
    public CrawlState(PlayerController controller) : base(controller) { }

    public override void OnEnter()
    {
        Debug.Log("Entering Crawl State (in movimento)");
        _controller.Animator.SetBool("isCrawling", true);
    }

    public override void OnUpdate()
    {
        if (_controller.CrouchInputDown)
        {
            _controller.ChangeState(_controller.locomotionState);
            return;
        }

        // ------- al momento voglio rimanere giù anche se non mi muovo --------

        //if (_controller.MovementInput.sqrMagnitude < 0.01f)
        //{
        //    _controller.ChangeState(_controller.crouchState);
        //    return;
        //}
        bool isMoving = _controller.MovementInput.sqrMagnitude > 0.01f;
        if (isMoving)
        {
            _controller.Animator.speed = 1f;
        }
        else
        {
            _controller.Animator.speed = 0f;
        }

        HandleCrawlMovement(_controller.MovementInput);
    }

    public override void OnExit()
    {
        _controller.Animator.SetBool("isCrawling", false);
    }

    private void HandleCrawlMovement(Vector2 movementInput)
    {
        Vector3 cameraForward = _controller.MainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = _controller.MainCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 moveDirection = Vector3.ClampMagnitude(cameraForward * movementInput.y + cameraRight * movementInput.x, 1f);


        _controller.Agent.velocity = moveDirection * _controller.CrawlSpeed;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            _controller.transform.rotation = Quaternion.Slerp(_controller.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
