using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using Unity.AI.Navigation;

public class Door : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float _targetYPosition = -4.5f; 
    [SerializeField] private float _moveDuration = 2.0f;

    [SerializeField] private NavMeshSurface _navMeshSurfaceToUpdate;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private bool _isOpen = false;
    private bool _isMoving = false;

    public static event Action OnDoorOpened;


    private void Awake()
    {
        _startPosition = transform.position;
        _endPosition = new Vector3(transform.position.x, _targetYPosition, transform.position.z);
    }

    public void ToggleDoor()
    {
        if (_isMoving) return;

        StopAllCoroutines();
        if (_isOpen)
            StartCoroutine(CloseDoor());
        else
            StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor()
    {
        _isMoving = true;
        _isOpen = true;
        Vector3 targetPosition = _isOpen ? _endPosition : _startPosition;
        Vector3 currentPos = transform.position;

        float elapsedTime = 0f;
        while (elapsedTime < _moveDuration)
        {
            transform.position = Vector3.Lerp(currentPos, targetPosition, elapsedTime / _moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        if (_isOpen)
        {
            gameObject.SetActive(false);
            Debug.Log("Porta disattivata.");

            if (_navMeshSurfaceToUpdate != null)
            {
                Debug.Log("Aggiornamento NavMesh in corso...");
                _navMeshSurfaceToUpdate.BuildNavMesh();
                Debug.Log("NavMesh aggiornata!");
            }
            OnDoorOpened?.Invoke();
            Debug.Log("Evento OnDoorOpened invocato!");
        }

        _isMoving = false;
    }

    private IEnumerator CloseDoor()
    {
        _isMoving = true;
        _isOpen = false; 

        Vector3 currentPos = transform.position;
        Vector3 targetPosition = _startPosition;   
        float elapsedTime = 0f;

        while (elapsedTime < _moveDuration)
        {
            transform.position = Vector3.Lerp(currentPos, targetPosition, elapsedTime / _moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; 

        if (_navMeshSurfaceToUpdate != null)
        {
            Debug.Log("Aggiornamento NavMesh in corso...");
            _navMeshSurfaceToUpdate.BuildNavMesh();
            Debug.Log("NavMesh aggiornata!");
        }

        _isMoving = false;
    }
}