using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class VrLogger : MonoBehaviour
{
    [SerializeReference]private List<LogType> _logTypes = new List<LogType>();
    [SerializeField]private int _numberOfLines = 10;

    private readonly Queue<string> _logQueue = new Queue<string>();
    private TextMeshPro _tmp;

    private void Awake()
    {
        _tmp = GetComponentInChildren<TextMeshPro>();
        Application.logMessageReceived += Handlelog;
    }

    private void OnEnable()
    {
        _tmp.text = String.Join("\n", _logQueue);
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= Handlelog;
    }

    private void Handlelog(string logString,string stackTrace, LogType type)
    {
        if (!_logTypes.Contains(type)) return;

        _logQueue.Enqueue(logString);

        if (_logQueue.Count > _numberOfLines)
        {
            _logQueue.Dequeue();
        }
        if (isActiveAndEnabled)
        {
            _tmp.text = String.Join("\n", _logQueue);
        }
    }
}
