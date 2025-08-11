using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{
    //reference variables
    [SerializeField] private Transform[] _waypoints;
    public NavMeshAgent Agent { get; private set; }
    public Transform PlayerTransform { get; private set; }

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
    public float SearchTime => _searchTime;

    //States
    private GuardBaseState _currentState;
    public PatrolState patrolState;
    public ChaseState chaseState;
    public SearchingState searchingState;
    public IdleState idleState;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _startingPosition = transform.position;
        _startingRotation = transform.rotation;
        patrolState = new PatrolState(this);
        chaseState = new ChaseState(this);
        searchingState = new SearchingState(this);
        idleState = new IdleState(this);
    }

    private void Start()
    {
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
        Collider[] playersInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _playerMask);
        if (playersInViewRadius.Length > 0)
        {
            Transform target = playersInViewRadius[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < _viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleMask))
                {
                    // Player is visible
                    return true;
                }
            }
        }
        return false;
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
    public void CatchPlayer()
    {
        Debug.Log("Game Over");
        //probabilmente da rivedere quando avrò una barra della vita per il giocatore
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);
        Vector3 viewAngleA = DirFromAngle(-_viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(_viewAngle / 2, false);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * _viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * _viewRadius);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
