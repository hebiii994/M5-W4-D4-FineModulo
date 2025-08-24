using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConeRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private int _segments = 50;
    [SerializeField] private LayerMask _obstacleMask;

    public float ViewRadius;
    public float ViewAngle;
    void Awake()
    {
        if (_lineRenderer == null)
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }
    }


    public void ToggleRenderer(bool isEnabled)
    {
        if (_lineRenderer != null)
        {
            _lineRenderer.enabled = isEnabled;
        }
    }
    void Start()
    {
        if (DebugManager.Instance != null)
        {
            DebugManager.Instance.RegisterCone(this);
            ToggleRenderer(DebugManager.Instance.IsDebugModeEnabled);
        }
        _lineRenderer.positionCount = _segments + 1;
    }


    private void OnDisable()
    {
        if (DebugManager.Instance != null)
        {
            DebugManager.Instance.UnregisterCone(this);
        }
    }

    void Update()
    {
        if (_lineRenderer != null && _lineRenderer.enabled)
        {
            DrawCone();
        }
    }

    void DrawCone()
    {
        Vector3[] points = new Vector3[_segments + 1];
        points[0] = Vector3.zero;
        float currentAngle = -ViewAngle / 2;
        float angleStep = ViewAngle / (_segments - 1);
        for (int i = 0; i < _segments; i++)
        {
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            RaycastHit hit;
            float x = Mathf.Sin(Mathf.Deg2Rad * currentAngle) * ViewRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * currentAngle) * ViewRadius;
            points[i + 1] = new Vector3(x, 0, z);
            if (Physics.Raycast(transform.position, direction, out hit, ViewRadius, _obstacleMask))
            {
                points[i + 1] = transform.InverseTransformPoint(hit.point);
            }
            else
            {
                points[i + 1] = transform.InverseTransformDirection(direction * ViewRadius);
            }
            currentAngle += angleStep;
        }

        _lineRenderer.SetPositions(points);
    }
}
