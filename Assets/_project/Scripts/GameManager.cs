using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _alertTimerText;
    public static GameManager Instance { get; private set; }

    [SerializeField] private AudioSource _ambientAudioSource;
    [SerializeField] private AudioSource _alertAudioSource;
    [SerializeField] private float _musicFadeDuration = 2.0f;

    private Coroutine _musicFadeCoroutine;

    private void OnEnable()
    {
        AlertManager.OnAlertStatusChanged += HandleAlertStatusChanged;
    }

    private void OnDisable()
    {
        AlertManager.OnAlertStatusChanged -= HandleAlertStatusChanged;
    }

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

    private void HandleAlertStatusChanged(bool isAlerted)
    {
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
