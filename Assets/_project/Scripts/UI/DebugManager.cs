using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugManager : MonoBehaviour
{
    private OnScreenConsole _console;
    public static DebugManager Instance { get; private set; }

    public bool IsDebugModeEnabled { get; private set; }

    private List<VisionConeRenderer> _activeCones = new List<VisionConeRenderer>();

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _console = FindObjectOfType<OnScreenConsole>(true);
        if (_console != null)
        {
            _console.TogglePanel(IsDebugModeEnabled);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _console = FindObjectOfType<OnScreenConsole>(true);
        if (_console != null)
        {
            _console.TogglePanel(IsDebugModeEnabled);
        }
    }
    public void RegisterCone(VisionConeRenderer cone)
    {
        if (!_activeCones.Contains(cone))
        {
            _activeCones.Add(cone);
        }
    }

    public void UnregisterCone(VisionConeRenderer cone)
    {
        if (_activeCones.Contains(cone))
        {
            _activeCones.Remove(cone);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            IsDebugModeEnabled = !IsDebugModeEnabled;

            Debug.Log($"Debug Mode: {(IsDebugModeEnabled ? "ATTIVATO" : "DISATTIVATO")}");

            if (_console != null)
            {
                _console.TogglePanel(IsDebugModeEnabled);
            }

            foreach (var cone in _activeCones)
            {
                cone.ToggleRenderer(IsDebugModeEnabled);
            }
        }
    }
}