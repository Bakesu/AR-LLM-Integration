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
using TMPro;

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
    TextMeshProUGUI promptAnswerText;

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
        //keywordRecognitionSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<KeywordRecognitionSubsystem>();
        dictationSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<DictationSubsystem>();

        //if (keywordRecognitionSubsystem != null)
        //{
        //    keywordRecognitionSubsystem.CreateOrGetEventForKeyword("Hey Assistant").AddListener(() => SwitchToDictation());
        //    Debug.Log("keyword system is running = " + keywordRecognitionSubsystem.running);
        //}

        if (dictationSubsystem != null)
        {
            // Add event handlers to all dictation subsystem events. 
            dictationSubsystem.Recognizing += Dictation_Recognizing;
            dictationSubsystem.RecognitionFaulted += Dictation_RecognitionFaulted;
            dictationSubsystem.Recognized += Dictation_Recognized;
            //dictationSubsystem.RecognitionFinished += Dictation_RecognitionFinished;
            Debug.Log("Dictation system is running = " + dictationSubsystem.running);
        }
        //dictationSubsystem.Stop();
    }

    private void Dictation_Recognizing(DictationResultEventArgs args)
    {
        i++;
        switch (i)
        {
            case 1:
                promptAnswerText.text = "Recognizing";
                break;
            case 2:
                promptAnswerText.text = "Recognizing.";
                break;
            case 3:
                promptAnswerText.text = "Recognizing..";
                i = 0;
                break;
        }
    }

    private void Dictation_RecognitionFaulted(DictationSessionEventArgs args)
    {
        Debug.Log(args.ReasonString);
        promptButton.GetComponent<ButtonDisabler>().EnableButton();
    }

    private void Dictation_Recognized(DictationResultEventArgs args)
    {
        StartCoroutine(objectHighlighter.ClearAllHighlights());
        debugWindow.Clear();

        dictationResult = args.Result + "?";
        dictationSubsystem.StopDictation();
        Debug.Log("Sending prompt:" + "'" + args.Result + "'");
        requestHandler.CreateFunctionCallRequest(dictationResult);
        //StartCoroutine(SwitchToKeywordRecognition());
    }

    //private void OnDisable()
    //{
    //    keywordRecognitionSubsystem.Stop();
    //    dictationSubsystem.Stop();
    //    dictationSubsystem.Recognizing -= Dictation_Recognizing;
    //    dictationSubsystem.Recognized -= Dictation_Recognized;
    //    dictationSubsystem.RecognitionFaulted -= Dictation_RecognitionFaulted;
    //}
    //public void SwitchToDictation()
    //{
    //    Debug.Log("SwitchToDictation");
    //    keywordRecognitionSubsystem.Stop();

    //    dictationSubsystem.Start();

    //    StopAllCoroutines();
    //    debugWindow.Clear();
    //    speechOutput.OnDictation();
    //    dictationSubsystem.StartDictation();
    //}

    public void StartDictation()
    {
        //StopAllCoroutines();
        debugWindow.Clear();
        speechOutput.OnDictation();
        dictationSubsystem.StartDictation();

        //HardCodedPrompt();
    }

    //private IEnumerator SwitchToKeywordRecognition()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    dictationSubsystem.Stop();

    //    keywordRecognitionSubsystem.Start();
    //    var keywords = keywordRecognitionSubsystem.GetAllKeywords();
    //    //Debug.Log(keywordRecognitionSubsystem.GetAllKeywords());
    //    foreach (var keyword in keywords)
    //    {
    //        Debug.Log(keyword.Key);
    //    }
    //    Debug.Log("Switched to KeywordRecognition");
    //}



    private void HardCodedPrompt()
    {
        StartCoroutine(objectHighlighter.ClearAllHighlights());
        var hardcodedPrompt = "Where should the CPU be placed on the motherboard?";
        dictationResult = hardcodedPrompt;
        requestHandler.CreateFunctionCallRequest(hardcodedPrompt);
    }
}
