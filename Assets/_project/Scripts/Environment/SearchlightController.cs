using UnityEngine;
using System.Linq;

public class SearchlightController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _sweepAngle = 90f;
    [SerializeField] private float _sweepSpeed = 0.5f;
    [SerializeField] private bool _invertDirection = false;

    [Header("Detection")]
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private LayerMask _obstacleMask;

    [Header("Colors")]
    [SerializeField] private Color _defaultColor = Color.yellow;
    [SerializeField] private Color _alertColor = Color.red;

    private Light _spotlight;
    private Transform[] _playerTargets;
    private Quaternion _initialRotation;

    void Start()
    {
        _spotlight = GetComponentInChildren<Light>();
        _initialRotation = transform.localRotation;
        if (_spotlight != null)
        {
            _spotlight.color = _defaultColor;
        }
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _playerTargets = playerObject.GetComponentsInChildren<Transform>()
                .Where(t => t.name.StartsWith("TargetPoint")).ToArray();
        }
    }

    void Update()
    {
        HandleRotation();
        HandleDetection();
    }

    private void HandleRotation()
    {
        float direction = _invertDirection ? -1f : 1f;
        float angleOffset = (_sweepAngle / 2) * Mathf.Sin(Time.time * _sweepSpeed) * direction;
        transform.localRotation = _initialRotation * Quaternion.Euler(0, angleOffset, 0);
    }

    private void HandleDetection()
    {
        if (IsPlayerInSight())
        {
            AlertManager.BroadcastAlert(_playerTargets[0].position);
            _spotlight.color = _alertColor;
        }
        else
        {
            _spotlight.color = _defaultColor;
        }
    }

    private bool IsPlayerInSight()
    {
        if (_playerTargets == null || _playerTargets.Length == 0 || _spotlight == null) return false;

        Transform lightTransform = _spotlight.transform;

        foreach (Transform target in _playerTargets)
        {
            float distanceToTarget = Vector3.Distance(lightTransform.position, target.position);

            if (distanceToTarget <= _spotlight.range)
            {
                Vector3 directionToTarget = (target.position - lightTransform.position).normalized;

                if (Vector3.Angle(lightTransform.forward, directionToTarget) < _spotlight.spotAngle / 2)
                {
                    if (!Physics.Raycast(lightTransform.position, directionToTarget, distanceToTarget, _obstacleMask))
                    {
                        Debug.DrawRay(lightTransform.position, directionToTarget * distanceToTarget, Color.green);
                        return true;
                    }
                    else
                    {
                        Debug.DrawRay(lightTransform.position, directionToTarget * distanceToTarget, Color.yellow);
                    }
                }
            }
        }

        Debug.DrawRay(lightTransform.position, lightTransform.forward * _spotlight.range, Color.red);
        return false;
    }
}