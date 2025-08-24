using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private OnScreenConsole _console;
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