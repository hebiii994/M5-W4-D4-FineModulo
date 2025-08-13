using Unity.IO.LowLevel.Unsafe;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GuardAI : MonoBehaviour
{
    //reference variables
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private Transform _playerTarget; 

    public Transform PlayerTarget { get; private set; }

    public Transform[] Waypoints => _waypoints;

    public NavMeshAgent Agent { get; private set; }
    public Transform PlayerTransform { get; private set; }
   

    public Animator Animator { get; private set; }

    //Waypoint patrol variables
    private int _currentWaypointIndex = 0;
    private bool _isPatrollingForward = true;
    public enum BehaviorType { Patrol, Stationary }
    [SerializeField] private BehaviorType _behaviorType = BehaviorType.Patrol;
    [SerializeField] private float _patrolSpeed = 3.5f;
    [SerializeField] private float _chaseSpeed = 7f;
    [SerializeField] private float _catchDistance = 1.5f;
    public BehaviorType CurrentBehaviorType => _behaviorType;

    public float PatrolSpeed => _patrolSpeed;
    public float ChaseSpeed => _chaseSpeed;
    public float CatchDistance => _catchDistance;
    public Vector3 LastKnownPlayerPosition { get; set; }

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

    //miscellaneous variables
    [SerializeField] private float _stopDistance = 1.5f; 
    [SerializeField] private float _damageAmount = 25;
    [SerializeField] private float _attackRate = 1f;
    private float _lastDamageTime;



    public float DamageAmount => _damageAmount;
    public float AttackRate => _attackRate;
    public float LastDamageTime => _lastDamageTime;

    //States
    private GuardBaseState _currentState;
    public PatrolState patrolState;
    public ChaseState chaseState;
    public SearchingState searchingState;
    public IdleState idleState;
    public FallState fallState;
    public SideHitState sideHitState;
    public LookAroundState lookAroundState;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponentInChildren<Animator>();
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _startingPosition = transform.position;
        _startingRotation = transform.rotation;
        patrolState = new PatrolState(this);
        chaseState = new ChaseState(this);
        searchingState = new SearchingState(this);
        idleState = new IdleState(this);
        fallState = new FallState(this);
        sideHitState = new SideHitState(this);
        lookAroundState = new LookAroundState(this);
    }
   
    private void Start()
    {
        _visionConeRenderer.ViewAngle = _viewAngle;
        _visionConeRenderer.ViewRadius = _viewRadius;
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

    public void ChangeState(GuardBaseState newState)
    {
        _currentState?.OnExit();    
        _currentState = newState;
        _currentState.OnEnter();    
    }
    public bool IsPlayerInSight()
    {
        Collider[] playersInViewRadius = Physics.OverlapSphere(_visionConeOrigin.position, _viewRadius, _playerMask);
        if (playersInViewRadius.Length > 0)
        {
            Transform playerRoot = playersInViewRadius[0].transform;
            Transform targetPoint = playerRoot.Find("TargetPoint");

            if (targetPoint == null)
            {
                targetPoint = playerRoot;
            }

            Vector3 directionToTarget = (targetPoint.position - _visionConeOrigin.position).normalized;
            if (Vector3.Angle(_visionConeOrigin.forward, directionToTarget) < _viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(_visionConeOrigin.position, targetPoint.position);
                if (!Physics.Raycast(_visionConeOrigin.position, directionToTarget, distanceToTarget, _obstacleMask))
                {
                    return true; 
                }
            }
        }
        return false;
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


    public void GetHit(int comboStep)
    {

        if (_currentState == fallState || _currentState == sideHitState) return;


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
    }

    public void UpdateLastDamageTime()
    {
        _lastDamageTime = Time.time;
    }

    public void HandleAttackRangeStop()
    {
        if (_playerTarget == null) return;

        float distance = Vector3.Distance(transform.position, _playerTarget.position);

        if (distance <= _stopDistance)
        {
            Agent.isStopped = true;
            Agent.updatePosition = false;
            Agent.updateRotation = false;
        }
        else
        {
            Agent.isStopped = false;
            Agent.updatePosition = true;
            Agent.updateRotation = true;
        }
    }

    public void OnLookAroundFinished()
    {
       
        if (CurrentBehaviorType == BehaviorType.Stationary)
        {
            ChangeState(idleState);
        }
        else
        {
            ChangeState(patrolState);
        }
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
