using TMPro;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // --- STATI DELLA FSM ---
    private PlayerBaseState _currentState;
    public LocomotionState locomotionState;
    public WallPressState wallPressState;
    public PlayerAttackState attackState;
    public CrouchState crouchState;
    public CrawlState crawlState;


    // --- RIFERIMENTI AI COMPONENTI ---
    public NavMeshAgent Agent { get; private set; }
    public Animator Animator { get; private set; }
    public Camera MainCamera { get; private set; }
    public PlayerCombat Combat { get; private set; }
    public CapsuleCollider CapsuleCollider { get; private set; }
    public bool CanMove { get; set; } = true;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    public float MoveSpeed => _moveSpeed;

    [Header("Crouch & Crawl")] 
    [SerializeField] private float _crawlSpeed = 2.0f;
    [SerializeField] private float _normalHeight = 1.8f;
    [SerializeField] private float _crouchHeight = 1.0f;
    public float CrawlSpeed => _crawlSpeed;
    public float NormalHeight => _normalHeight;
    public float CrouchHeight => _crouchHeight;

    [Header("Interaction")]
    [SerializeField] private float _interactionDistance = 2f;
    [SerializeField] private float _interactionSphereRadius = 0.5f;
    [SerializeField] private float _interactionHeight = 1f;
    [SerializeField] private TextMeshProUGUI _interactionPromptText;

    public float InteractionDistance => _interactionDistance;
    public float InteractionSphereRadius => _interactionSphereRadius;
    public float InteractionHeight => _interactionHeight;
    public PlayerBaseState CurrentState => _currentState;

    //proprietà per input
    public Vector2 MovementInput { get; private set; }
    public bool CrouchInputDown { get; private set; }
    public bool AttackInputDown { get; private set; }
    public bool WallPressInputDown { get; private set; }

    [Header("Wall Press")]
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _wallSlideSpeed = 3f;
    [SerializeField] private GameObject _wallPressCamera;
    public LayerMask WallLayer => _wallLayer;
    public float WallSlideSpeed => _wallSlideSpeed;
    public GameObject WallPressCamera => _wallPressCamera;

    private IInteractable _focusedInteractable;

    private bool _acceptingInput = false;
    private void Awake()
    {

        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponentInChildren<Animator>();
        MainCamera = Camera.main;
        Combat = GetComponent<PlayerCombat>();
        CapsuleCollider = GetComponent<CapsuleCollider>();

        locomotionState = new LocomotionState(this);
        wallPressState = new WallPressState(this);
        attackState = new PlayerAttackState(this);
        crouchState = new CrouchState(this);
        crawlState = new CrawlState(this);
    }

    private void Start()
    {
        Agent.speed = _moveSpeed;
        Agent.acceleration = 999f;
        Agent.angularSpeed = 999f;

        ChangeState(locomotionState);
        StartCoroutine(EnableInputAfterDelay(0.1f));
    }

    private void Update()
    {
        HandleInputs();
        if (!_acceptingInput) return;
        _currentState?.OnUpdate();
    }

    public void ChangeState(PlayerBaseState newState)
    {
        _currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter();
    }

    private IEnumerator EnableInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _acceptingInput = true;
    }
    private void HandleInputs()
    {

        MovementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        CrouchInputDown = Input.GetKeyDown(KeyCode.C);
        AttackInputDown = Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(1);
        WallPressInputDown = Input.GetKeyDown(KeyCode.Space);
    }

    public void SetFocus(IInteractable newInteractable)
    {
        if (newInteractable == _focusedInteractable) return;
        _focusedInteractable = newInteractable;
        _interactionPromptText.text = _focusedInteractable.InteractionPrompt;
        _interactionPromptText.gameObject.SetActive(true);
    }

    public void ClearFocus()
    {
        if (_focusedInteractable != null)
        {
            _focusedInteractable = null;
            _interactionPromptText.gameObject.SetActive(false);
        }
    }

    public void InteractWithFocused()
    {
        _focusedInteractable?.Interact();
    }

    public void StopMovement()
    {
        if (Agent.enabled)
        {
            Agent.velocity = Vector3.zero;
            Agent.ResetPath();
        }
    }

    public void OnAttackFinished()
    {
        // Se siamo in PlayerAttackState, notifichiamogli che l'animazione è finita.
        if (_currentState is PlayerAttackState attackState)
        {
            attackState.OnAnimationFinished();
        }
    }

    // --- DEBUG ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 startPoint = transform.position + Vector3.up * _interactionHeight;
        Vector3 endPoint = startPoint + transform.forward * _interactionDistance;
        Gizmos.DrawWireSphere(startPoint, _interactionSphereRadius);
        Gizmos.DrawWireSphere(endPoint, _interactionSphereRadius);
    }
}