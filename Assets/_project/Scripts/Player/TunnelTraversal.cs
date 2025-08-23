using UnityEngine;

public class TunnelTraversal : MonoBehaviour
{
    [SerializeField] private Transform _exitPoint;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                if (player.CurrentState is CrawlState || player.CurrentState is CrouchState)
                {
                    Debug.Log("Inizio la traversata del cunicolo!");
                    player.StartCoroutine(player.TraverseTunnel(_exitPoint.position));
                }
            }
        }
    }
}