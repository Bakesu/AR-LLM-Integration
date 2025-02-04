using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Vuforia;


public class FunctionCallHandler : MonoBehaviour
{
    [SerializeField]
    private ObjectHighlighter objectHighlighter;

    [SerializeField]
    private ImageCapture imageCapture;

    [SerializeField]
    private RequestHandler requestHandler;

    [SerializeField]
    private SpeechOutput speechOutput;
    [SerializeField]
    internal Texture2D hardcodedImage;

    [SerializeField]
    TextMeshProUGUI promptAnswerText;
    // this flag is used in coroutine image requests to make sure everything is finished before showing to the user
    internal byte[] imageAsPNG;
    private List<string> labelList;

    private void Start()
    {
        imageAsPNG = hardcodedImage.EncodeToPNG();
    }

    internal void HighlightLabels(ExtractedLabelData extractedLabelData)
    {
        objectHighlighter.HighlightLabels(extractedLabelData.Label);
        Debug.Log("label: " + String.Join(", ", extractedLabelData.Label) + " + TextContent: " + extractedLabelData.TextContent);
        //promptAnswerText.text = extractedLabelData.TextContent;
        //labelList = extractedLabelData.Label;
        speechOutput.OnSpeak(extractedLabelData.TextContent);
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
        speechOutput.OnSpeak(answer);
        //promptAnswerText.text = answer;
    }

    public void CaptureImage(string FCArgument)
    {
        Debug.Log("CaptureImage called");        
        StartCoroutine(imageCapture.CaptureImageAndSendIt());
        //TODO: test on headset
    }

    public void GiveInstructions(string FCArgument)
    {
        InstructionsObject instructionsObject = JsonConvert.DeserializeObject<InstructionsObject>(FCArgument);       
              
        StartCoroutine(DrawRelation(instructionsObject.assemblingObject, instructionsObject.placeableObject));
    }

    public IEnumerator DrawRelation(string subject, string placeableObject)
    {
        string labelPrompt = "Please provide the labels for the following object: " + subject;
        requestHandler.CreateImageRequest(labelPrompt, imageAsPNG, true);

        yield return new WaitForSeconds(4);
        //Highlight objects, relevant labels and create an arrow between them - createimagerequest makes the call to highlight labels
        //TODO - How do we get the labels and use them for the arrow?
        objectHighlighter.CreateArrowObject(subject, placeableObject);        
        objectHighlighter.HighlightObject(placeableObject);
    }
}
