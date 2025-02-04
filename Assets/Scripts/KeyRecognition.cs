using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyRecognition : MonoBehaviour
{
    [SerializeField]
    ObjectHighlighter objectHighlighter;
    [SerializeField]
    SpeechInput speechInput;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // Highlight Motherboard
        {
            Debug.Log("Highlight Motherboard");
            objectHighlighter.ClearAllHighlights();
            objectHighlighter.HighlightObject("x1");
        }
        if (Input.GetKeyDown(KeyCode.C)) // Highlight CPU
        {
            Debug.Log("Highlight CPU");
            objectHighlighter.ClearAllHighlights();
            objectHighlighter.HighlightObject("x2");
        }
        if (Input.GetKeyDown(KeyCode.F)) // Highlight Fan
        {
            Debug.Log("Highlight Fan");
            objectHighlighter.ClearAllHighlights();
            objectHighlighter.HighlightObject("x3");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Highlight from CPU to MB");
            objectHighlighter.ClearAllHighlights();
            objectHighlighter.CreateArrowObject("x2", "x1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Highlight from Fan to MB");
            objectHighlighter.ClearAllHighlights();
            objectHighlighter.CreateArrowObject("x3", "x1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("Clear all highlights");
            objectHighlighter.ClearAllHighlights();
        }
    }
}