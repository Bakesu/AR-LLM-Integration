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
    TextMeshProUGUI promptAnswerText;

    internal string sceneComponentList;

    public void Start()
    {
        sceneComponentList = CreateComponentList();
    }

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
        //TODO: make CreateImageRequest() call
    }



    //Creates component list based on the children of the objectHighlighter Gameobject
    internal string CreateComponentList()
    {
        var sceneObjectList = "[";
        foreach (var qrObjectPair in objectHighlighter.imageTargets)
        {
            string listAppend = qrObjectPair.Key + ":" + qrObjectPair.Value.gameObject.name + ", ";
            sceneObjectList = string.Concat(sceneObjectList, listAppend);
        }
        char[] charsToTrim = { ',', ' ' };
        sceneObjectList = sceneObjectList.TrimEnd(charsToTrim);
        sceneObjectList = string.Concat(sceneObjectList, "]");
        return sceneObjectList;
    }
}
