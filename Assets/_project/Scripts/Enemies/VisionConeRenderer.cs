using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConeRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private int _segments = 50;

    public float ViewRadius;
    public float ViewAngle;
    void Start()
    {
        if (_lineRenderer == null)
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }
        _lineRenderer.positionCount = _segments + 1;
    }

   
    void Update()
    {
        DrawCone();
    }

    void DrawCone()
    {
        Vector3[] points = new Vector3[_segments + 1];
        points[0] = Vector3.zero;
        float currentAngle = -ViewAngle / 2;
        float angleStep = ViewAngle / (_segments - 1);
        for (int i = 0; i < _segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * currentAngle) * ViewRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * currentAngle) * ViewRadius;
            points[i + 1] = new Vector3(x, 0, z);
            currentAngle += angleStep;
        }

        _lineRenderer.SetPositions(points);
    }
}
