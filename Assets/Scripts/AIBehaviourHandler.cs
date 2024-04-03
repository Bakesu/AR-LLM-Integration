using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class AIBehaviourHandler : MonoBehaviour
{
    [SerializeField] 
    private ObjectHighlighter objectHighlighter;

    [SerializeField]
    TextMeshProUGUI promptAnswerText;
    public void highlight_objects(string parameters)
    {
        Debug.Log("highlighted_object yipeee");
    }

    internal void HighlightLabels(LabelExtractedData extractedData)
    {
        try
        {
            objectHighlighter.HighlightLabels(extractedData.Label);
            Debug.Log("label: " + String.Join(", ", extractedData.Label) + " + TextContent: " + extractedData.TextContent);
            promptAnswerText.text = extractedData.TextContent;
        }
        catch (Exception e)
        {
            Debug.Log("No labels were given " + e);
        }
    }
}
