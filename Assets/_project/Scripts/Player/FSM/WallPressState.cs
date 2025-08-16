// In WallPressState.cs

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

        if (Physics.Raycast(_controller.transform.position + Vector3.up * _controller.InteractionHeight,_controller.transform.forward,out RaycastHit wallHit,1.5f,_controller.WallLayer))
        {
            _wallNormal = wallHit.normal;


            _controller.transform.position = new Vector3(wallHit.point.x, _controller.transform.position.y, wallHit.point.z) + _wallNormal * 0.3f;
            _controller.transform.rotation = Quaternion.LookRotation(_wallNormal);
        }

        if (_controller.WallPressCamera != null)
        {
            _controller.WallPressCamera.SetActive(true);
        }
    }

    public override void OnUpdate()
    {

        if (_controller.WallPressInputDown)
        {
            _controller.ChangeState(_controller.locomotionState);
            return;
        }

        Vector3 checkOrigin = _controller.transform.position + Vector3.up * _controller.InteractionHeight;
        if (!Physics.Raycast(checkOrigin, -_controller.transform.forward, 1.1f, _controller.WallLayer))
        {
            _controller.ChangeState(_controller.locomotionState);
            return;
        }

        HandleWallMovement();
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

        float horizontalInput = _controller.MovementInput.x;
        _controller.Animator.SetFloat("WallMoveDirection", horizontalInput);
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            Vector3 movementDirection = Vector3.Cross(_wallNormal, Vector3.up);
            Vector3 moveDelta = movementDirection * horizontalInput * _controller.WallSlideSpeed * Time.deltaTime;
            Vector3 nextPosition = _controller.Rigidbody.position + moveDelta;
            _controller.Rigidbody.MovePosition(nextPosition);
        }
    }
}