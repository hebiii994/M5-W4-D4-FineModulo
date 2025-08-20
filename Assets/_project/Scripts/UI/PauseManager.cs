using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private GameObject _ExitoptionsButton;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        _pauseMenuPanel.SetActive(true);
        _optionsPanel.SetActive(false);
        _ExitoptionsButton.SetActive(false);
        Time.timeScale = 0f;


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        _pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenOptions()
    {

        foreach (Transform child in _pauseMenuPanel.transform)
        {
            if (child.gameObject != _optionsPanel) 
            {
                child.gameObject.SetActive(false);
            }
        }
        _optionsPanel.SetActive(true);
        _ExitoptionsButton.SetActive(true);
    }

    public void CloseOptions()
    {
        _optionsPanel.SetActive(false);
        foreach (Transform child in _pauseMenuPanel.transform)
        {
            child.gameObject.SetActive(true);
        }
        _ExitoptionsButton.SetActive(false);
        _optionsPanel.SetActive(false);
    }

    public void QuitToMainMenu()
    {

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }
}