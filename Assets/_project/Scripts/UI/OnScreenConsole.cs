using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class OnScreenConsole : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _logText;
    [SerializeField] private int _maxLines = 20;

    private Queue<string> _logQueue = new Queue<string>();

    private void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }
    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    public void TogglePanel(bool isEnabled)
    {
        gameObject.SetActive(isEnabled);
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (_logQueue == null) return;

        _logQueue.Enqueue(logString);
        while (_logQueue.Count > _maxLines)
        {
            _logQueue.Dequeue();
        }

        if (_logText != null)
        {
            _logText.text = string.Join("\n", _logQueue);
        }
    }
}