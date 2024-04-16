using Newtonsoft.Json;
using System;
using System.Collections;
using TMPro;
using UnityEngine;


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
    // this flag is used in coroutine image requests to make sure everything is finished before showing to the user
    internal bool followupimageRequestIsFinished = false;
    internal Queue taskQueue = new Queue();

    internal void HighlightLabels(ExtractedLabelData extractedLabelData)
    {
        objectHighlighter.HighlightLabels(extractedLabelData.Label);
        Debug.Log("label: " + String.Join(", ", extractedLabelData.Label) + " + TextContent: " + extractedLabelData.TextContent);
        promptAnswerText.text = extractedLabelData.TextContent;
        followupimageRequestIsFinished = true;
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
        
        string labelPrompt = "Please provide the labels for the following object: " + instructionsObject.assemblingObject;
        var imageAsPNG = hardcodedImage.EncodeToPNG();

        StartCoroutine(GetInstructionObjectLabel(labelPrompt, imageAsPNG, instructionsObject.assemblingObject, instructionsObject.placeableObject));
    }

    public IEnumerator GetInstructionObjectLabel(string labelPrompt, byte[] imageAsPNG, string subject, string placeableObject)
    {
        followupimageRequestIsFinished = false;
        requestHandler.CreateImageRequest(labelPrompt, imageAsPNG, true);

        yield return new WaitUntil(() => followupimageRequestIsFinished == true);
        Debug.Log("draw arrow would be here");
        //Labels are highlighted by the previous image request
        objectHighlighter.HighlightObject(placeableObject);
        
        //objectHighlighter.HighlightObject(placeableObject);


    }
}
