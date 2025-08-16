using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _alertTimerText;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null )
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (_alertTimerText != null)
        {
            _alertTimerText.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        AlertManager.Tick(Time.deltaTime);
        UpdateAlertUI();
    }
    private void UpdateAlertUI()
    {
        if (_alertTimerText == null) return; 

        
        if (AlertManager.IsAlertActive)
        {
            _alertTimerText.gameObject.SetActive(true);
            int timeLeft = Mathf.CeilToInt(AlertManager.AlertTimer);
            _alertTimerText.text = $"ALERT\n{timeLeft}";
        }

        else
        {
            _alertTimerText.gameObject.SetActive(false);
        }
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER! Ricarico la scena...");
        SceneManager.LoadScene("GameOver");
    }

    public void Victory()
    {
        Debug.Log("VITTORIA! Ritorno al menù principale...");
        // posso aggiungere suoni o un recap del livello in futuro
        SceneManager.LoadScene("MainMenu");
    }
}
