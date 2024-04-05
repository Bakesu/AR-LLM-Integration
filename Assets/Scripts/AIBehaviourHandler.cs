using System;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;


public class AIBehaviourHandler : MonoBehaviour
{
    [SerializeField]
    private ObjectHighlighter objectHighlighter;

    [SerializeField]
    private ImageCapture imageCapture;

    [SerializeField]
    TextMeshProUGUI promptAnswerText;

    internal void HighlightLabels(ExtractedLabelData extractedLabelData)
    {
        objectHighlighter.HighlightLabels(extractedLabelData.Label);
        Debug.Log("label: " + String.Join(", ", extractedLabelData.Label) + " + TextContent: " + extractedLabelData.TextContent);
        promptAnswerText.text = extractedLabelData.TextContent;
    }

    public void HighlightObjects(string componentName)
    {
        componentName = DataUtility.ExtractFunctionArgumentsFromFCString(componentName);
        
        if(objectHighlighter.imageTargets.ContainsKey(componentName))
        {            
            objectHighlighter.HighlightObject(componentName);
        }
        else
        {
            Debug.Log("Component not found");
        }
        //string[] labels = parameters.Split(',');
        //objectHighlighter.highlightObject();
    }

    public void TextualAnswer(string FCArgument)
    {
        var answer = DataUtility.ExtractFunctionArgumentsFromFCString(FCArgument);
        promptAnswerText.text = answer;
    }

    public void CaptureImage(string FCArgument)
    {
        Debug.Log("CaptureImage was called: ");
        imageCapture.CaptureImageAndSendIt();
        //TODO: make CreateImageRequest() call
    }
}
