using UnityEngine;
using System.Linq;
using System.Collections;

public class SecurityCamera : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float _viewRadius = 15f;
    [Range(0, 360)]
    [SerializeField] private float _viewAngle = 120f;
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private Transform _visionConeOrigin;
    [SerializeField] private VisionConeRenderer _visionConeRenderer;

    private Transform[] _playerTargets;
    private bool _isActivated = false; 

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            var targets = playerObject.GetComponentsInChildren<Transform>();
            _playerTargets = targets.Where(t => t.name.StartsWith("TargetPoint")).ToArray();
        }

        if (_visionConeRenderer != null)
        {
            _visionConeRenderer.ViewAngle = _viewAngle;
            _visionConeRenderer.ViewRadius = _viewRadius;
        }

        StartCoroutine(ActivationRoutine());
    }

    private IEnumerator ActivationRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        _isActivated = true;
    }

    void Update()
    {
        if (!_isActivated) return;

        HandleDetection();
    }

    private void HandleDetection()
    {
        if (IsPlayerInSight())
        {
            AlertManager.TriggerAlert();
        }
        else
        {

        }
    }

    public bool IsPlayerInSight()
    {
        if (!_isActivated) return false;

        if (_playerTargets == null || _playerTargets.Length == 0) return false;

        foreach (Transform targetPoint in _playerTargets)
        {
            float distanceToTarget = Vector3.Distance(_visionConeOrigin.position, targetPoint.position);
            if (distanceToTarget > _viewRadius)
            {
                continue;
            }


            Vector3 directionToTarget = (targetPoint.position - _visionConeOrigin.position).normalized;
            if (Vector3.Angle(_visionConeOrigin.forward, directionToTarget) < _viewAngle / 2)
            {
                if (!Physics.Raycast(_visionConeOrigin.position, directionToTarget, distanceToTarget, _obstacleMask))
                {
                    return true;
                }
            }
        }

        return false;
    }
}