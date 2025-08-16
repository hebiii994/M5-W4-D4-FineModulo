using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class AlertManager
{
    public static event Action<bool> OnAlertStatusChanged;
    public static float AlertTimer { get; private set; }

    // Constants
    private const float ALERT_DURATION = 99.9f;
    private const float GRACE_PERIOD = 5.0f;
    private const float FAST_DRAIN_MULTIPLIER = 10f;

    //provo a rendere più intelligente il sistema di allerta
    private static List<GuardAI> _activeChasers = new List<GuardAI>();


    // Public properties
    public static bool IsAlertActive => AlertTimer > 0;

    // Private variables
    private static float _lastTimePlayerWasSeen;

    public static void Tick(float deltaTime)
    {
        if (AlertTimer > 0)
        {
            float drainRate = 1.0f;
            if (_activeChasers.Count == 0 && Time.time - _lastTimePlayerWasSeen > GRACE_PERIOD)
            {
                drainRate = FAST_DRAIN_MULTIPLIER;
                Debug.Log("Nessun avvistamento recente, fase di cautela");
            }

            AlertTimer -= deltaTime * drainRate;

            if (AlertTimer <= 0)
            {
                AlertTimer = 0;
                Debug.Log("ALLARME TERMINATO.");
                OnAlertStatusChanged?.Invoke(false);
            }
        }
    }

    public static void TriggerAlert()
    {
        if (AlertTimer <= 0)
        {
            Debug.Log("ALLARME ATTIVATO!");
            OnAlertStatusChanged?.Invoke(true);
        }
        AlertTimer = ALERT_DURATION;
        _lastTimePlayerWasSeen = Time.time;
    }

    public static void RegisterChaser(GuardAI guard)
    {
        if (!_activeChasers.Contains(guard))
        {
            _activeChasers.Add(guard);
        }
        _lastTimePlayerWasSeen = Time.time; 
    }

    public static void UnregisterChaser(GuardAI guard)
    {
        if (_activeChasers.Contains(guard))
        {
            _activeChasers.Remove(guard);
        }
    }

    public static void ReportPlayerSeen()
    {
        if (IsAlertActive)
        {
            _lastTimePlayerWasSeen = Time.time;
        }
    }
}


