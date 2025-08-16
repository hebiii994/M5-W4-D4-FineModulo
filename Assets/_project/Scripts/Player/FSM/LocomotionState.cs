using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionState : PlayerBaseState
{
    public enum MovementMode { WASD, PointAndClick }
    private MovementMode _currentMode = MovementMode.WASD;

    public LocomotionState(PlayerController controller) : base(controller) { }
    public override void OnEnter()
    {
        _controller.Agent.enabled = true;
        Debug.Log("Entering Locomotion State");
        _controller.Animator.SetBool("isCrouching", false);
        _controller.CapsuleCollider.height = _controller.NormalHeight;
        _controller.CapsuleCollider.center = new Vector3(0, _controller.NormalHeight / 2f, 0);
    }

    public override void OnUpdate()
    {
        //transazione ad attacco
        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            _controller.ChangeState(_controller.attackState);
            return;
        }
        //transazione ad interazione muro
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            if (CheckForWall())
            {
                _controller.ChangeState(_controller.wallPressState);
                return;
            }
        }

        //transazione ad accovacciamento
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            _controller.ChangeState(_controller.crouchState);
            return;
        }

        //transazione tipo di movimento
        if (Input.GetKeyDown(KeyCode.T))
        {
            _currentMode = (_currentMode == MovementMode.WASD) ? MovementMode.PointAndClick : MovementMode.WASD;
            _controller.Agent.ResetPath();
            Debug.Log("Movement mode changed to: " + _currentMode);
        }

        switch (_currentMode)
        {
            case MovementMode.WASD:
                HandleWASDMovement();
                break;
            case MovementMode.PointAndClick:
                HandlePointAndClickMovement();
                break;
        }

        HandleInteractionCheck();

        //interazione con oggetti
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            _controller.InteractWithFocused();
        }

        UpdateAnimator();
    }

    public override void OnExit()
    {

        if (_controller.Agent.enabled)
        {
            _controller.Agent.velocity = Vector3.zero; 
            _controller.Agent.ResetPath();
        }
        Debug.Log("Exiting Locomotion State");
    }

    private void HandleWASDMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 cameraForward = _controller.MainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = _controller.MainCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal);
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        if (_controller.CanMove)
        {
            _controller.Agent.velocity = moveDirection * _controller.MoveSpeed;

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                _controller.transform.rotation = Quaternion.Slerp(_controller.transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }

    private void HandlePointAndClickMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _controller.MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (_controller.CanMove)
                {
                    _controller.Agent.SetDestination(hit.point);
                }
            }
        }
    }

    private bool CheckForWall()
    {
        Vector3 origin = _controller.transform.position + Vector3.up * _controller.InteractionHeight;
        Vector3 direction = _controller.transform.forward;
        float distance = 1f; 

        return Physics.Raycast(origin, direction, distance, _controller.WallLayer);
    }


    private void HandleInteractionCheck()
    {
        Vector3 origin = _controller.transform.position + Vector3.up * _controller.InteractionHeight;
        float radius = _controller.InteractionSphereRadius;
        Vector3 direction = _controller.transform.forward;
        float distance = _controller.InteractionDistance;

        if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit, distance))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                _controller.SetFocus(interactable);
                return;
            }
        }

        _controller.ClearFocus();
    }

    private void UpdateAnimator()
    {
        float speed = _controller.Agent.velocity.magnitude;
        _controller.Animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
    }


}
