using TMPro;
using UnityEngine;

public class DebugWindow : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    // Use this for initialization
    void Start()
    {
        textMesh = gameObject.GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        Application.logMessageReceived += LogMessage;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogMessage;
    }

    public void LogMessage(string message, string stackTrace, LogType type)
    {
        if (textMesh == null) return;
        if (textMesh.text.Length > 300)
        {
            textMesh.text = message + "\n";
        }
        else
        {
            textMesh.text += message + "\n";
        }
    }

    public void Clear()
    {
        textMesh.text = "";
    }   
}