using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEngine.Assertions.Must;

public class SpeechInput : MonoBehaviour
{
    DictationSubsystem dictationSubsystem;

    [SerializeField]
    DebugWindow debugWindow;

    [SerializeField]
    RequestHandler requestHandler;

    [SerializeField]
    ObjectHighlighter objectHighlighter;

    [SerializeField]
    SpeechOutput speechOutput;

    [SerializeField]
    GameObject promptButton;

    [SerializeField]
    Texture2D hardcodedImage;

    ImageCapture imageCapture;
    private KeywordRecognitionSubsystem keywordRecognitionSubsystem;

    internal string dictationResult;

    private int i = 0;
    void Start()
    {
        imageCapture = GetComponentInParent<ImageCapture>();

        // Get the first running dictation subsystem.
        dictationSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<DictationSubsystem>();
        keywordRecognitionSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<KeywordRecognitionSubsystem>();

        if (keywordRecognitionSubsystem != null)
        {
            // Register a keyword and its associated action with the subsystem
            keywordRecognitionSubsystem.CreateOrGetEventForKeyword("Hey Assistant").AddListener(() => DisableKeywordRecognition()); ;
            keywordRecognitionSubsystem.CreateOrGetEventForKeyword("Hey Assistant").AddListener(() => StartDictation()); ;
        }

        if (dictationSubsystem != null)
        {            
            // Add event handlers to all dictation subsystem events. 
            dictationSubsystem.Recognizing += Dictation_Recognizing;
            dictationSubsystem.Recognized += Dictation_Recognized;
            //dictationSubsystem.RecognitionFinished += Dictation_RecognitionFinished;
            dictationSubsystem.RecognitionFaulted += Dictation_RecognitionFaulted;

        }
        dictationSubsystem.Stop();
    }

    private void OnDisable()
    {
        dictationSubsystem.Recognizing -= Dictation_Recognizing;
        dictationSubsystem.Recognized -= Dictation_Recognized;
        dictationSubsystem.RecognitionFaulted -= Dictation_RecognitionFaulted;
    }
    private void Dictation_RecognitionFaulted(DictationSessionEventArgs args)
    {
        Debug.Log(args.ReasonString);
        promptButton.GetComponent<ButtonDisabler>().EnableButton();
    }

    private void Dictation_RecognitionFinished(DictationSessionEventArgs args)
    {
        
    }

    private void Dictation_Recognized(DictationResultEventArgs args)
    {
        StartCoroutine(objectHighlighter.ClearAllHighlights());
        debugWindow.Clear();
        dictationResult = args.Result + "?";
        Debug.Log("Sending prompt:" + "'" + args.Result + "'");
        dictationSubsystem.StopDictation();

        //If photocapture is set, we want to pull it down before creating a new one
        if (imageCapture.photoCaptureObject != null)
        {
            Debug.Log("Disposing of old photoCaptureObject");
            imageCapture.photoCaptureObject.Dispose();
            imageCapture.photoCaptureObject = null;
        }

        requestHandler.CreateFunctionCallRequest(dictationResult);        
        
        StartCoroutine(DisableDictation());
    }

    private IEnumerator DisableDictation()
    {
        Debug.Log("Dictation finished");
        dictationSubsystem.Stop();
        yield return new WaitForSeconds(1);
        EnableKeywordRecognition();
        StopCoroutine(DisableDictation());
        Debug.Log("shouldnt be seen?");
    }

    private void Dictation_Recognizing(DictationResultEventArgs args)
    {
        i++;
        switch (i)
        {
            case 1:
                Debug.Log("Recognizing");
                break;
            case 2:
                Debug.Log("Recognizing.");
                break;
            case 3:
                Debug.Log("Recognizing..");
                i = 0;
                break;
        }
    }

    public void StartDictation()
    {
        dictationSubsystem.StartDictation();
        speechOutput.OnDictation();
        debugWindow.Clear();
        StopAllCoroutines();
        //Clear all highlights and prompt text
        //StopAllCoroutines();
        //StartCoroutine(objectHighlighter.ClearAllHighlights());

        //Harcoded prompt for testing
        //var hardcodedPrompt = "Where should the CPU be placed on the motherboard?";
        //dictationResult = hardcodedPrompt;
        //requestHandler.CreateFunctionCallRequest(hardcodedPrompt);        
    }

    public void DisableKeywordRecognition()
    {
        keywordRecognitionSubsystem.Stop();

        Debug.Log("keywordRecognitionSubsystem is running " + keywordRecognitionSubsystem.running);
    }

    public void EnableKeywordRecognition()
    {
        keywordRecognitionSubsystem.Start();
        Debug.Log("keywordRecognitionSubsystem is running " + keywordRecognitionSubsystem.running);
    }
}
