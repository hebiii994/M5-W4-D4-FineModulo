using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform; 
    private Transform _cameraTransform;

    private float _heightOffset = 50f;

    void Awake()
    {
        _cameraTransform = transform;
    }

    void LateUpdate()
    {
        if (_playerTransform == null) return;

        Vector3 targetPosition = new Vector3(
            _playerTransform.position.x,
            _heightOffset,
            _playerTransform.position.z
        );

        _cameraTransform.position = targetPosition;
    }
}