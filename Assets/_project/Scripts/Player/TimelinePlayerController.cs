using UnityEngine;

public class TimelinePlayerController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject _keyPanel;

    private void Awake()
    {
        if (_playerController == null)
        {
            _playerController = GetComponent<PlayerController>();
        }
    }

    public void DisableControl()
    {
        if (_playerController != null)
        {
            _playerController.enabled = false;
            Debug.Log("Controllo del giocatore DISABILITATO dalla Timeline.");
        }
    }
    public void EnableControl()
    {
        if (_playerController != null)
        {
            _playerController.enabled = true;
            if (_keyPanel != null)
            {
                _keyPanel.SetActive(true);
            }
            Debug.Log("Controllo del giocatore ABILITATO dalla Timeline.");
        }
    }
}