using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _pressStartPanel; 
    [SerializeField] private GameObject _mainMenuPanel; 
    [SerializeField] private GameObject _firstSelectedButton;
    private GameObject _lastSelectedGameObject;

    //variabili per i suoni del menu
    [SerializeField] private AudioClip _startSound;       
    [SerializeField] private AudioClip _navigateSound;    
    [SerializeField] private AudioClip _confirmSound;     
    [SerializeField] private AudioClip _exitSound;        

    private AudioSource _audioSource;
    private bool _mainMenuIsActive = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        _pressStartPanel.SetActive(true);
        _mainMenuPanel.SetActive(false);
    }


    void Update()
    {
        if (!_mainMenuIsActive && Input.GetButtonDown("Submit"))
        {
            PlaySound(_startSound);
            ShowMainMenu();
        }

        if (_mainMenuIsActive)
        {
            if (EventSystem.current.currentSelectedGameObject != _lastSelectedGameObject)
            {
                if (EventSystem.current.currentSelectedGameObject != null)
                {
                    PlaySound(_navigateSound);
                }
                _lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    private void ShowMainMenu()
    {
        _mainMenuIsActive = true;
        _pressStartPanel.SetActive(false);
        _mainMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstSelectedButton);
        _lastSelectedGameObject = _firstSelectedButton;
    }

    public void StartGame()
    {
        PlaySound(_confirmSound);
        Debug.Log("Avvio del gioco...");
        SceneManager.LoadScene("Level_01");
    }
    public void Options()
    {
        PlaySound(_confirmSound);
        // Qui in futuro ci sarà la logica per aprire il menù opzioni
        Debug.Log("Menù Opzioni non ancora implementato.");
    }

    public void QuitGame()
    {
        PlaySound(_confirmSound);
        Debug.Log("Chiusura del gioco...");

            Application.Quit();

    }

}
