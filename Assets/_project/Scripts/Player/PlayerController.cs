using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public enum MovementMode
    {
        PointAndClick,
        WASD
    }

    [SerializeField] private MovementMode _currentMode = MovementMode.PointAndClick;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private PlayerCombat _playerCombat;
    public bool CanMove { get; set; } = true;

    //interaction variables
    [SerializeField] private float _interactionDistance = 2f;
    [SerializeField] private float _interactionSphereRadius = 0.5f;
    [SerializeField] private float _interactionHeight = 1f; 
    [SerializeField] private TextMeshProUGUI _interactionPromptText;

    //private variables
    private NavMeshAgent _agent;
    private Camera _mainCamera;
    private Rigidbody _rb;
    private IInteractable _focusedInteractable;

    //wall press variables
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _wallCheckDistance = 1f;
    [SerializeField] private float _wallSlideSpeed = 8f;
    [SerializeField] private float _characterShoulderOffset = 0.4f;
    [SerializeField] private GameObject _wallPressCamera;
    private Coroutine _wallCoroutine;
    private bool _isAgainstWall = false;
    private Vector3 _wallNormal;

    public bool IsAgainstWall => _isAgainstWall;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _mainCamera = Camera.main;
        _rb = GetComponent<Rigidbody>();
        _rb.interpolation = RigidbodyInterpolation.None;
        _rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        _rb.isKinematic = true;
    }

    private void Start()
    {
        _agent.speed = _moveSpeed;
        _agent.acceleration = 999f;
        _agent.angularSpeed = 999f;

        if (_interactionPromptText != null)
            _interactionPromptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_playerCombat != null && _playerCombat.IsAttacking)
        {
            StopMovement();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            ToggleWallMode();
        }
        if (!_isAgainstWall)
        {
            HandleInput();
            HandleMovement();
            HandleInteractionCheck();
        }

    }

    private void ToggleWallMode()
    {
        if (_isAgainstWall)
        {
            if (_wallCoroutine != null) StopCoroutine(_wallCoroutine);
            _isAgainstWall = false;
            _agent.enabled = true; 
            if (_wallPressCamera != null) _wallPressCamera.SetActive(false);
        }
        else if (CheckForWall(out RaycastHit hit))
        {
            _wallNormal = hit.normal;
            _wallCoroutine = StartCoroutine(WallPressRoutine(hit));
        }
    }
    private IEnumerator WallPressRoutine(RaycastHit wallHit)
    {
        _isAgainstWall = true;
        _agent.velocity = Vector3.zero;
        _agent.enabled = false;
        if (_wallPressCamera != null) _wallPressCamera.SetActive(true);

        transform.rotation = Quaternion.LookRotation(wallHit.normal);
        transform.position = new Vector3(wallHit.point.x, transform.position.y, wallHit.point.z) + wallHit.normal * 0.3f;

        while (_isAgainstWall)
        {
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                if (_playerCombat != null && !_playerCombat.IsAttacking)
                {
                    _playerCombat.KnockOnWall();
                }
            }

            float horizontalInput = Input.GetAxis("Horizontal");
            if (Mathf.Abs(horizontalInput) > 0.1f)
            {
                Vector3 movementDirection = Vector3.Cross(_wallNormal, Vector3.up);
                Vector3 moveDelta = movementDirection * horizontalInput * _wallSlideSpeed * Time.deltaTime;
                Vector3 nextPosition = transform.position + moveDelta;
                Vector3 centerCheckOrigin = nextPosition + Vector3.up * 0.5f;
                bool hasWallInFront = Physics.Raycast(centerCheckOrigin, -_wallNormal, 0.5f, _wallLayer);
                Vector3 shoulderOffset = movementDirection * Mathf.Sign(horizontalInput) * _characterShoulderOffset;
                Vector3 leadingEdgeCheckOrigin = centerCheckOrigin + shoulderOffset;
                bool hasWallOnMovingSide = Physics.Raycast(centerCheckOrigin + shoulderOffset, -_wallNormal, 0.5f, _wallLayer);

                if (hasWallInFront && hasWallOnMovingSide)
                {
                    _rb.MovePosition(nextPosition);
                }
            }

            yield return null; 
        }
    }
    private bool CheckForWall(out RaycastHit hitInfo)
    {
        Vector3 checkOrigin = transform.position + Vector3.up * _interactionHeight;
        return Physics.Raycast(checkOrigin, transform.forward, out hitInfo, _wallCheckDistance, _wallLayer);
    }


    private void HandleInteractionCheck()
    {
        Vector3 interactionOrigin = transform.position + Vector3.up * _interactionHeight;
        if (Physics.SphereCast(interactionOrigin, _interactionSphereRadius, transform.forward, out RaycastHit hit, _interactionDistance))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                if (interactable != _focusedInteractable)
                {
                    _focusedInteractable = interactable;
                    ShowInteractionPrompt();
                    
                }
                return;
            } 
        }
        ClearFocus();
    }

    private void ShowInteractionPrompt()
    {
        if (_interactionPromptText != null && _focusedInteractable != null)
        {
            _interactionPromptText.text = _focusedInteractable.InteractionPrompt;
            _interactionPromptText.gameObject.SetActive(true);
        }
    }

    private void ClearFocus()
    {
        if (_focusedInteractable != null)
        {
            _focusedInteractable = null;
            if (_interactionPromptText != null)
                _interactionPromptText.gameObject.SetActive(false);
        }
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _currentMode = (_currentMode == MovementMode.PointAndClick) ? MovementMode.WASD : MovementMode.PointAndClick;
            Debug.Log("Modalità cambiata in: " + _currentMode);
            _agent.ResetPath();
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton0) && _focusedInteractable != null)
        {
            _focusedInteractable.Interact();
        }
    }

    private void HandleMovement()
    {
        if (!CanMove)
        {
            _agent.velocity = Vector3.zero;
            return; 
        }

        switch (_currentMode)
        {
            case MovementMode.PointAndClick:
                HandlePointAndClickMovement();
                break;
            case MovementMode.WASD:
                HandleWASDMovement();
                break;
        }
    }

    private void HandlePointAndClickMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _agent.SetDestination(hit.point);
            }
        }
    }
    private void HandleWASDMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 cameraForward = _mainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = _mainCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal);
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);
        Vector3 desiredVelocity = moveDirection * _moveSpeed;

        _agent.velocity = desiredVelocity;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
    public void StopMovement()
    {
        if (!_agent.enabled)
        {
            return; 
        }
        _agent.velocity = Vector3.zero;
        if (_agent.hasPath) 
        {
            _agent.ResetPath();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 startPoint = transform.position + Vector3.up * _interactionHeight;
        Vector3 endPoint = startPoint + transform.forward * _interactionDistance;
        Gizmos.DrawWireSphere(startPoint, _interactionSphereRadius);

        Gizmos.DrawWireSphere(endPoint, _interactionSphereRadius);

        Vector3 up = transform.up * _interactionSphereRadius;
        Vector3 right = transform.right * _interactionSphereRadius;

        Gizmos.DrawLine(startPoint - right, endPoint - right);
        Gizmos.DrawLine(startPoint + right, endPoint + right);
        Gizmos.DrawLine(startPoint - up, endPoint - up);
        Gizmos.DrawLine(startPoint + up, endPoint + up);
    }
}




