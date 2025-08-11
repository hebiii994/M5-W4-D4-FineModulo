using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;


public class Door : MonoBehaviour
{
    [SerializeField] private NavMeshSurface _navMeshSurface;
    public void OpenDoor()
    {
        // Logica per aprire la porta momentanea
        gameObject.SetActive(false);
        Debug.Log("Porta aperta!");
        if (_navMeshSurface != null)
        {
            _navMeshSurface.BuildNavMesh();
        }
    }
}
