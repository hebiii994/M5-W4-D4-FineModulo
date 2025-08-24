using System;
using System.Collections;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{
    //reference variables
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private Transform[] _playerTargets;

    //properties 
    public Transform[] Waypoints => _waypoints;
    public NavMeshAgent Agent { get; private set; }
    public Transform PlayerTransform => (_playerTargets != null && _playerTargets.Length > 0) ? _playerTargets[0] : null;
    public Animator Animator { get; private set; }

    public PlayerController PlayerController { get; private set; }

    //Waypoint patrol variables
    private int _currentWaypointIndex = 0;
    private bool _isPatrollingForward = true;
    public enum BehaviorType { Patrol, Stationary }
    [SerializeField] private BehaviorType _behaviorType = BehaviorType.Patrol;
    [SerializeField] private float _patrolSpeed = 3.5f;
    [SerializeField] private float _chaseSpeed = 7f;
    [SerializeField] private float _catchDistance = 1.5f;

    //properties for patrol and chase speeds
    public BehaviorType CurrentBehaviorType => _behaviorType;
    public float PatrolSpeed => _patrolSpeed;
    public float ChaseSpeed => _chaseSpeed;
    public float CatchDistance => _catchDistance;
    public Vector3 LastKnownPlayerPosition { get; set; }
    public float LastHitTime { get; private set; } = -99f; 
    public float StunDuration => _stunDurationAfterHit;


    //Idle variables
    private Vector3 _startingPosition;
    private Quaternion _startingRotation;
    [SerializeField] private float _idleWaitTime = 3f;
    public float IdleWaitTime => _idleWaitTime;
    public Vector3 StartingPosition => _startingPosition;
    public Quaternion StartingRotation => _startingRotation;

    //Angle Of Vision variables
    [SerializeField] private float _viewRadius = 10f;
    [Range(0, 360)]
    [SerializeField] private float _viewAngle = 90f;
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _searchTime = 5f;
    [SerializeField] private Transform _visionConeOrigin;
    [SerializeField] private VisionConeRenderer _visionConeRenderer;

    public float SearchTime => _searchTime;

    //alarm variables
    [SerializeField] private float _alertRadius = 15f;
    

    //combat variables
    [SerializeField] private float _stopDistance = 1.5f; 
    [SerializeField] private float _damageAmount = 20;
    [SerializeField] private float _attackRate = 1f;
    [SerializeField] private float _stunDurationAfterHit = 0.5f;
    private float _lastDamageTime;

    //health variables
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;
    private bool _isDead = false;


    public float AttackRate => _attackRate;
    public float LastDamageTime => _lastDamageTime;
    public bool IsDead => _isDead;

    //States
    private GuardBaseState _currentState;
    private float _lastStateChangeTime;
    private const float STATE_TRANSITION_COOLDOWN = 0.1f;
    public GuardBaseState CurrentState => _currentState;

    // public states
    public PatrolState patrolState;
    public ChaseState chaseState;
    public SearchingState searchingState;
    public IdleState idleState;
    public FallState fallState;
    public SideHitState sideHitState;
    public LookAroundState lookAroundState;
    public GetUpState getUpState;
    public AlertState alertState;
    public AttackState attackState;
    public DeadState deadState;


    private void OnEnable()
    {
        AlertManager.OnAlertPositionBroadcast += HandleAlert;
    }

    private void OnDisable()
    {
        AlertManager.OnAlertPositionBroadcast -= HandleAlert;
    }

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponentInChildren<Animator>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("ERRORE: Nessun GameObject con tag 'Player' trovato nella scena!", this);
            this.enabled = false;
            return;
        }
        var targets = playerObject.GetComponentsInChildren<Transform>();
        _playerTargets = targets.Where(t => t.name.StartsWith("TargetPoint")).ToArray();

        if (_playerTargets.Length == 0)
        {
            Debug.LogWarning("Attenzione: Nessun 'TargetPoint' trovato come figlio del Player. La guardia userà il transform principale del giocatore.", this);
            _playerTargets = new Transform[] { playerObject.transform };
        }

        PlayerController = _playerTargets[0].GetComponentInParent<PlayerController>();

        _startingPosition = transform.position;
        _startingRotation = transform.rotation;
        patrolState = new PatrolState(this);
        chaseState = new ChaseState(this);
        searchingState = new SearchingState(this);
        idleState = new IdleState(this);
        fallState = new FallState(this);
        sideHitState = new SideHitState(this);
        lookAroundState = new LookAroundState(this);
        getUpState = new GetUpState(this);
        alertState = new AlertState(this);
        attackState = new AttackState(this);
        deadState = new DeadState(this);

        _currentHealth = _maxHealth;
        _visionConeRenderer.ViewAngle = _viewAngle;
        _visionConeRenderer.ViewRadius = _viewRadius;
        Agent.stoppingDistance = _stopDistance;
        if (_behaviorType == BehaviorType.Patrol)
        {
            ChangeState(patrolState);
        }
        else if (_behaviorType == BehaviorType.Stationary)
        {
            ChangeState(idleState);
        }
    }
   

    private void Update()
    {
        _currentState?.OnUpdate();
    }

    public void ChangeState(GuardBaseState newState, bool force = false)
    {
        if (_currentState != null && Time.time - _lastStateChangeTime < STATE_TRANSITION_COOLDOWN && !force)
        {
            return;
        }
        if (!force)
        {
            if (_currentState != null && newState.Priority < _currentState.Priority)
            {
                return;
            }
        }
        Debug.Log($"{name}: State change from [{_currentState?.GetType().Name ?? "NONE"}] -> [{newState.GetType().Name}] (Forced: {force})");

        _currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter();

        _lastStateChangeTime = Time.time;
    }
    public bool IsPlayerInSight()
    {
        if (_playerTargets == null || _playerTargets.Length == 0) return false;

        Collider[] playersInViewRadius = Physics.OverlapSphere(_visionConeOrigin.position, _viewRadius, _playerMask);
        if (playersInViewRadius.Length > 0)
        {
            foreach (Transform targetPoint in _playerTargets)
            {
                Vector3 directionToTarget = (targetPoint.position - _visionConeOrigin.position).normalized;
                if (Vector3.Angle(_visionConeOrigin.forward, directionToTarget) < _viewAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(_visionConeOrigin.position, targetPoint.position);
                    if (!Physics.Raycast(_visionConeOrigin.position, directionToTarget, distanceToTarget, _obstacleMask))
                    {
                        Debug.Log("Guardia: Ho visto il player!");
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void ReturnToDefaultState()
    {
        if (CurrentBehaviorType == BehaviorType.Patrol)
        {
            ChangeState(patrolState, true);
        }
        else
        {
            ChangeState(idleState, true);
        }
    }

    public void SetCurrentWaypointIndex(int index) 
    {
        _currentWaypointIndex = index;
    }
    public void GoToNextWaypoint()
    {
        if (_waypoints.Length == 0) return;

        Agent.SetDestination(_waypoints[_currentWaypointIndex].position);

        if (_isPatrollingForward)
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= _waypoints.Length)
            {
                _currentWaypointIndex = _waypoints.Length - 2;
                _isPatrollingForward = false;
            }
        }
        else
        {
            _currentWaypointIndex--;
            if (_currentWaypointIndex < 0)
            {
                _currentWaypointIndex = 1;
                _isPatrollingForward = true;
            }

        }
    }

    public void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 1f, CatchDistance, _playerMask);

        if (hits.Length > 0)
        {
            PlayerHealth playerHealth = hits[0].GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                Debug.Log("<color=red>COLPO DELLA GUARDIA A SEGNO!</color> Danno inflitto.");
                playerHealth.TakeDamage((int)_damageAmount);
                UpdateLastDamageTime();
            }
        }
        else
        {
            Debug.Log("Guardia: ho attaccato ma il giocatore non era a portata.");
        }
    }

    private void HandleAlert(Vector3 alertPosition)
    {
        if (_isDead)
        {
            return;
        }

        if (_currentState == chaseState || _currentState == fallState || _currentState == sideHitState || _currentState == getUpState)
        {
            return;
        }
        if (Vector3.Distance(transform.position, alertPosition) <= _alertRadius)
        {
            Debug.Log(gameObject.name + " ha sentito l'allarme!");

            LastKnownPlayerPosition = alertPosition;

            ChangeState(alertState);
            Debug.Log($"{name} received alert! State={_currentState}, Priority={_currentState?.Priority}. PlayerInSight={IsPlayerInSight()}");

        }
        

    }
    public void BroadcastAlert()
    {
        Debug.Log(gameObject.name + " sta lanciando un allarme a tutte le altre unità!");
        AlertManager.BroadcastAlert(PlayerTransform.position);
    }

    public void GetHit(int comboStep, int damageAmount)
    {
        if (_isDead) return;
        if (_currentState is FallState || _currentState is SideHitState || _currentState is GetUpState) return;
        AlertManager.TriggerAlert();
        _currentHealth -= damageAmount;
        Debug.Log("Vita della guardia rimasta: " + _currentHealth);
        LastHitTime = Time.time;
        if (_currentHealth <= 0)
        {
            _isDead = true;
            Animator.SetTrigger("Die");
            ChangeState(deadState);
        }

        if (Agent != null) Agent.isStopped = true;

        if (comboStep > 2)
        {
            Debug.Log($"--- PRE-FALL --- Stato: {CurrentState.GetType().Name}, Posizione Agente: {Agent.nextPosition}, Posizione Transform: {transform.position}, Agente Attivo: {Agent.enabled}, Agente Stoppato: {Agent.isStopped}");
        }

        if (comboStep <= 2)
            {
                Animator.SetInteger("HitType", 1);
                ChangeState(sideHitState);
            }
            else
            {
                Animator.SetInteger("HitType", 2);
                ChangeState(fallState);
            }
        
        
        Debug.Log("<color=orange>GUARDIA DEBUG: Colpito! Ricevuto comboStep = " + comboStep + "</color>", this);

        
    }

    public void UpdateLastDamageTime()
    {
        _lastDamageTime = Time.time;
    }

  

    public void OnLookAroundFinished()
    {

        if (AlertManager.IsAlertActive)
        {
            Debug.Log("Allarme ancora attivo. Ritorno in AlertState per rivalutare.");
            ChangeState(alertState);
        }
        else
        {
            Debug.Log("Allarme terminato. Ritorno alla routine.");
            if (CurrentBehaviorType == BehaviorType.Stationary)
            {
                ChangeState(idleState);
            }
            else
            {
                ChangeState(patrolState);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"<color=yellow>TRIGGER DEBUG:</color> Oggetto '{gameObject.name}' ha rilevato una collisione con '{other.gameObject.name}' (Tag: {other.tag})", gameObject);

        if (_isDead)
        {
            Debug.Log("<color=grey>TRIGGER DEBUG:</color> Guardia morta, ignoro il trigger.", gameObject);
            return;
        }

        if (other.CompareTag("Noise"))
        {
            Debug.Log($"<color=lightblue>TRIGGER DEBUG:</color> È un rumore! Lo stato attuale della guardia è: {_currentState}", gameObject);

            if (_currentState is SearchingState || _currentState is ChaseState || _currentState is FallState || _currentState is SideHitState || _currentState is AttackState || _currentState is GetUpState)
            {
                Debug.Log("<color=orange>TRIGGER DEBUG:</color> Guardia già in stato attivo. Rumore ignorato.", gameObject);
                return;
            }

            Debug.Log("<color=green>TRIGGER DEBUG:</color> Guardia in stato tranquillo, REAGISCO al rumore!", gameObject);
            LastKnownPlayerPosition = other.transform.position;
            ChangeState(searchingState);
        }
    }

    private void OnDrawGizmos()
    {
        // Disegna la sfera di allarme per debug
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f); 
        Gizmos.DrawSphere(transform.position, _alertRadius);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(_visionConeOrigin.position, _viewRadius);
        Vector3 viewAngleA = DirFromAngle(-_viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(_viewAngle / 2, false);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_visionConeOrigin.position, _visionConeOrigin.position + viewAngleA * _viewRadius);
        Gizmos.DrawLine(_visionConeOrigin.position, _visionConeOrigin.position + viewAngleB * _viewRadius);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += _visionConeOrigin.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

   
}
