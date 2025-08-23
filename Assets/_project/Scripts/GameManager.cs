using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _minimapPanel;
    [SerializeField] private GameObject _alertPanel;
    [SerializeField] private TextMeshProUGUI _alertTimerText;
    public static GameManager Instance { get; private set; }

    [SerializeField] private AudioSource _ambientAudioSource;
    [SerializeField] private AudioSource _alertAudioSource;
    [SerializeField] private float _musicFadeDuration = 2.0f;

    private Coroutine _musicFadeCoroutine;
    private bool _sceneReferencesLoaded = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        AlertManager.OnAlertStatusChanged += HandleAlertStatusChanged;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        AlertManager.OnAlertStatusChanged -= HandleAlertStatusChanged;
    }

    private void Awake()
    {
        AlertManager.Reset();
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
        if (_minimapPanel != null) _minimapPanel.SetActive(true);
        if (_alertPanel != null) _alertPanel.SetActive(false);
    }
    private void Update()
    {
        if (_sceneReferencesLoaded && SceneManager.GetActiveScene().name == "Heliport")
        {
            AlertManager.Tick(Time.deltaTime);
            if (_sceneReferencesLoaded)
            {
                UpdateAlertUI();
            }
        }
    }
    private void UpdateAlertUI()
    {
        if (_alertPanel == null || !_alertPanel.activeSelf || _alertTimerText == null)
        {
            return;
        }
        if (_alertTimerText == null) return;
        if (!_alertPanel.activeSelf || _alertTimerText == null) return;
        float timer = AlertManager.AlertTimer;
        int seconds = Mathf.FloorToInt(timer);
        int milliseconds = Mathf.FloorToInt((timer - seconds) * 100);
        _alertTimerText.text = $"{seconds:D2}:{milliseconds:D2}";

    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Heliport")
        {
            AlertManager.Reset();
            FindSceneReferences();
        }
        else
        {
            if (_ambientAudioSource != null) _ambientAudioSource.Stop();
            if (_alertAudioSource != null) _alertAudioSource.Stop();
            _sceneReferencesLoaded = false;
            _minimapPanel = null;
            _alertPanel = null;
            _alertTimerText = null;
            _ambientAudioSource = null;
            _alertAudioSource = null;
        }
    }

    private void FindSceneReferences()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Transform minimapTransform = canvas.transform.Find("Minimap_Panel");
            if (minimapTransform != null) _minimapPanel = minimapTransform.gameObject;

            Transform alertTransform = canvas.transform.Find("Alert_Panel");
            if (alertTransform != null) _alertPanel = alertTransform.gameObject;
        }
            

        if (_alertPanel != null)
        {
            _alertTimerText = _alertPanel.GetComponentInChildren<TextMeshProUGUI>(true);
            _alertPanel.SetActive(false);
        }
        if (_minimapPanel != null)
        {
            _minimapPanel.SetActive(true);
        }


        GameObject ambientSourceObject = GameObject.FindWithTag("AmbientMusicSource");
        if (ambientSourceObject != null) _ambientAudioSource = ambientSourceObject.GetComponent<AudioSource>();

        GameObject alertSourceObject = GameObject.FindWithTag("AlertMusicSource");
        if (alertSourceObject != null) _alertAudioSource = alertSourceObject.GetComponent<AudioSource>();

        HandleAlertStatusChanged(false);
        _sceneReferencesLoaded = true;
        Debug.Log("Riferimenti della scena caricati correttamente.");
    }
    private void HandleAlertStatusChanged(bool isAlerted)
    {
        if (_minimapPanel != null) _minimapPanel.SetActive(!isAlerted);
        if (_alertPanel != null) _alertPanel.SetActive(isAlerted);
        if (_musicFadeCoroutine != null)
        {
            StopCoroutine(_musicFadeCoroutine);
        }

        if (isAlerted)
        {
            if (!_alertAudioSource.isPlaying)
            {
                _alertAudioSource.Play();
            }
            _musicFadeCoroutine = StartCoroutine(FadeMusic(_alertAudioSource, _ambientAudioSource));
        }
        else
        {
            if (!_ambientAudioSource.isPlaying)
            {
                _ambientAudioSource.Play();
            }
            _musicFadeCoroutine = StartCoroutine(FadeMusic(_ambientAudioSource, _alertAudioSource));
        }
    }

    private IEnumerator FadeMusic(AudioSource sourceToFadeUp, AudioSource sourceToFadeDown)
    {
        float timer = 0f;
        float startVolumeDown = sourceToFadeDown.volume;

        sourceToFadeUp.volume = 0f;

        while (timer < _musicFadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / _musicFadeDuration;

            sourceToFadeUp.volume = Mathf.Lerp(0f, 0.35f, progress);
            sourceToFadeDown.volume = Mathf.Lerp(startVolumeDown, 0f, progress);

            yield return null;
        }

        sourceToFadeUp.volume = 0.35f;
        sourceToFadeDown.volume = 0f;

        sourceToFadeDown.Stop();
    }

    public void GameOver()
    {
        if (_ambientAudioSource != null) _ambientAudioSource.Stop();
        if (_alertAudioSource != null) _alertAudioSource.Stop();

        Debug.Log("GAME OVER! Ricarico la scena...");
        SceneManager.LoadScene("GameOver");
    }

    public void Victory()
    {
        if (_ambientAudioSource != null) _ambientAudioSource.Stop();
        if (_alertAudioSource != null) _alertAudioSource.Stop();

        Debug.Log("VITTORIA! Ritorno al menù principale...");
        // posso aggiungere suoni o un recap del livello in futuro
        SceneManager.LoadScene("MainMenu");
    }
}
