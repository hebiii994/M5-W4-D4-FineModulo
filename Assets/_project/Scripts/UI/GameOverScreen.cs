using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private GameObject _pressContinuePanel;
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _firstSelectedButton;
    [SerializeField] private GameObject _animationimage;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _gameOverMusic;

    private string _gameLevelSceneName = "Level_01";
    private string _mainMenuSceneName = "MainMenu";

    void Awake()
    {
        if (_pressContinuePanel != null) _pressContinuePanel.SetActive(false);
        if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false);
        if (_animationimage != null) _animationimage.SetActive(true);
    }
    void Start()
    {
        if (_audioSource != null && _gameOverMusic != null)
        {
            _audioSource.PlayOneShot(_gameOverMusic);
        }
    }

    public void OnTextAnimationFinished()
    {
        if (_pressContinuePanel != null)
            _pressContinuePanel.SetActive(true);

        if (_mainMenuPanel != null)
            _mainMenuPanel.SetActive(true);

        StartCoroutine(SetFirstSelectedButton());

    }

    private IEnumerator SetFirstSelectedButton()
    {
        yield return null;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstSelectedButton);
    }

    public void Continue()
    {
        SceneManager.LoadScene(_gameLevelSceneName);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(_mainMenuSceneName);
    }
}
