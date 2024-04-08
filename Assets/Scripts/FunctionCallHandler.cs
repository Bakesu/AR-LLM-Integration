using Newtonsoft.Json;
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using static Microsoft.MixedReality.GraphicsTools.Editor.MeasureToolSettings;


public class FunctionCallHandler : MonoBehaviour
{
    [SerializeField]
    private ObjectHighlighter objectHighlighter;

    [SerializeField]
    private ImageCapture imageCapture;

    [SerializeField]
    private RequestHandler requestHandler;

    [SerializeField]
    internal Texture2D hardcodedImage;

    [SerializeField]
    TextMeshProUGUI promptAnswerText;
    private bool imageRequestIsRunning = false;

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
        //TODO: test on headset
    }

    public void GiveInstructions(string FCArgument)
    {        
        InstructionsObject instructionsObject = JsonConvert.DeserializeObject<InstructionsObject>(FCArgument);       
        objectHighlighter.HighlightObject(instructionsObject.placeableObject);
        string labelPrompt = "Please provide the labels for the following object: " + instructionsObject.assemblingObject;
        var imageAsPNG = hardcodedImage.EncodeToPNG();        
        var labels = GetInstructionObjectLabel(labelPrompt, imageAsPNG);
        while (imageRequestIsRunning)
        {
            
        }
        Debug.Log("task done " + labels);
        //instructionsObject.placeableObject
        Debug.Log("assemblingObject = " + instructionsObject.assemblingObject + "placeableObject = " + instructionsObject.placeableObject);        
    }

    public string GetInstructionObjectLabel(string labelPrompt, byte[] imageAsPNG)
    {
        imageRequestIsRunning = true;
        requestHandler.CreateImageRequest(labelPrompt, imageAsPNG, true);
        return "done";
        
    }
}
