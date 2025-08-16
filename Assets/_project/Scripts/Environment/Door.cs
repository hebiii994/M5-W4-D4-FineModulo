using UnityEngine;
using UnityEngine.AI; // Necessario per NavMeshObstacle
using System;

public class Door : MonoBehaviour
{
    private NavMeshObstacle _obstacle;
    public static event Action OnDoorOpened;

    private void Awake()
    {
        _obstacle = GetComponent<NavMeshObstacle>();
    }

    public void OpenDoor()
    {
        if (_obstacle != null)
        {
            _obstacle.enabled = false;
        }

        gameObject.SetActive(false);

        OnDoorOpened?.Invoke();
        Debug.Log("Porta aperta e ostacolo NavMesh rimosso!");
    }
}