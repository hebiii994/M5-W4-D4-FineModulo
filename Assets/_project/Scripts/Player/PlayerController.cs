using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public enum MovementMode
    {
        PointAndClick,
        WASD
    }

    [SerializeField] private MovementMode _currentMode = MovementMode.PointAndClick;
    [SerializeField] private float _moveSpeed = 5f;
    public bool CanMove { get; set; } = true;

    //interaction variables
    [SerializeField] private float _interactionDistance = 2f;
    [SerializeField] private float _interactionSphereRadius = 0.5f;
    [SerializeField] private float _interactionHeight = 1f; 
    [SerializeField] private TextMeshProUGUI _interactionPromptText;

    private NavMeshAgent _agent;
    private Camera _mainCamera;
    private Rigidbody _rb;

    private IInteractable _focusedInteractable;

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
        if (!CanMove)
        {
            StopMovement();
            return;
        }

        HandleInput();
        HandleMovement();
        HandleInteractionCheck();
        

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
        if (Input.GetKeyDown(KeyCode.E) && _focusedInteractable != null)
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

        _agent.velocity = Vector3.zero;
        _agent.ResetPath();
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




