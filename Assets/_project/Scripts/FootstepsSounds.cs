using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsSounds : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _normalFootsteps;
    [SerializeField] private AudioClip[] _waterFootsteps;

    [SerializeField] private float _raycastDistance = 1.2f;
    [SerializeField] private LayerMask _terrainLayer;

    [SerializeField] private float _minTimeBetweenSteps = 0.25f;
    private float _lastStepTime;
    void Awake()
    {
        
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayFootstepSound()
    {
        if (Time.time - _lastStepTime < _minTimeBetweenSteps)
        {
            return;
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource non è stato trovato o assegnato");
            return; 
        }

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _raycastDistance, _terrainLayer))
        {
            AudioClip[] clipsToPlay = null;

            switch (hit.collider.tag)
            {
                case "Terrain_Water":
                    clipsToPlay = _waterFootsteps;
                    Debug.Log("Passo su una pozzanghera!");
                    break;
                case "Terrain_Normal":
                    clipsToPlay = _normalFootsteps;
                    break;
                default:
                    clipsToPlay = _normalFootsteps; 
                    break;
            }

            if (clipsToPlay != null && clipsToPlay.Length > 0)
            {
                AudioClip randomClip = clipsToPlay[Random.Range(0, clipsToPlay.Length)];
                if (randomClip != null)
                {
                    _audioSource.PlayOneShot(randomClip);
                    _lastStepTime = Time.time;
                }
                else
                {
                    Debug.LogWarning("L'array di suoni contiene un elemento null");
                }
            }
        }
    }
}
