using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPressState : PlayerBaseState
{
    private Vector3 _wallNormal;

    public WallPressState(PlayerController controller) : base(controller) { }
    public override void OnEnter()
    {
        Debug.Log("Entering Wall Press State");

        _controller.Agent.velocity = Vector3.zero;
        _controller.Agent.enabled = false;

        _controller.Animator.SetBool("isAgainstWall", true);

        if (Physics.Raycast(_controller.transform.position + Vector3.up, _controller.transform.forward, out RaycastHit wallHit, 1f, _controller.WallLayer))
        {
            _wallNormal = wallHit.normal;
            _controller.transform.rotation = Quaternion.LookRotation(-_wallNormal);
            _controller.transform.position = wallHit.point + _wallNormal * 0.3f;
        }

        if (_controller.WallPressCamera != null)
        {
            _controller.WallPressCamera.SetActive(true);
        }
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            _controller.ChangeState(_controller.locomotionState);
            return;
        }

        if (!Physics.Raycast(_controller.transform.position + Vector3.up, -_controller.transform.forward, 1f, _controller.WallLayer))
        {
            _controller.ChangeState(_controller.locomotionState);
            return;
        }

        HandleWallMovement();

        // Input per bussare sul muro
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton3)) 
        {
            _controller.Combat.KnockOnWall();
        }
    }

    public override void OnExit()
    {
        Debug.Log("Exiting Wall Press State");
        _controller.Agent.enabled = true;
        _controller.Animator.SetBool("isAgainstWall", false);
        _controller.Animator.SetFloat("WallMoveDirection", 0f);

        if (_controller.WallPressCamera != null)
        {
            _controller.WallPressCamera.SetActive(false);
        }
    }

    private void HandleWallMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 movementDirection = Vector3.Cross(_wallNormal, Vector3.up);
        Vector3 moveDelta = movementDirection * -horizontalInput * _controller.WallSlideSpeed * Time.deltaTime;
        _controller.transform.position += moveDelta;
        _controller.Animator.SetFloat("WallMoveDirection", horizontalInput);
    }
}

