using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    [SerializeField] private float _victoryDelay = 1.0f;
    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;
        if (other.CompareTag("Player"))
        {

            _hasTriggered = true;
            StartCoroutine(VictorySequence());
        }
    }

    private IEnumerator VictorySequence()
    {
        yield return new WaitForSeconds(_victoryDelay);
        GameManager.Instance.Victory();
    }
}
