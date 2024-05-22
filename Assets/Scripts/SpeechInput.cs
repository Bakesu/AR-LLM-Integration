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

    internal string dictationResult;

    private int i = 0;
    void Start()
    {
        imageCapture = GetComponentInParent<ImageCapture>();

        // Get the first running dictation subsystem.
        dictationSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<DictationSubsystem>();
        dictationSubsystem.Start();

        if (dictationSubsystem != null)
        {
            // Add event handlers to all dictation subsystem events. 
            dictationSubsystem.Recognizing += Dictation_Recognizing;
            dictationSubsystem.Recognized += Dictation_Recognized;
            //dictationSubsystem.RecognitionFinished += Dictation_RecognitionFinished;
            dictationSubsystem.RecognitionFaulted += Dictation_RecognitionFaulted;
            Debug.Log("Dictation system is running = " + dictationSubsystem.running);
        }
        //dictationSubsystem.Stop();
    }

    private void Dictation_RecognitionFaulted(DictationSessionEventArgs args)
    {
        Debug.Log(args.ReasonString);
        promptButton.GetComponent<ButtonDisabler>().EnableButton();
    }

    private void Dictation_RecognitionFinished(DictationSessionEventArgs args)
    {
        Debug.Log("Dictation finished");
    }

    private void Dictation_Recognized(DictationResultEventArgs args)
    {
        StartCoroutine(objectHighlighter.ClearAllHighlights());
        debugWindow.Clear();

        dictationResult = args.Result + "?";
        dictationSubsystem.StopDictation();
        speechOutput.OnSpeak("Sending prompt");
        //Debug.Log("Sending prompt:" + "'" + args.Result + "'");
        StartCoroutine(imageCapture.CaptureImageAndSendIt());
        //StartCoroutine(SwitchToKeywordRecognition());

        //If photocapture is set, we want to pull it down before creating a new one
        //if (imageCapture.photoCaptureObject != null)
        //{
        //    Debug.Log("Disposing of old photoCaptureObject");
        //    imageCapture.photoCaptureObject.Dispose();
        //    imageCapture.photoCaptureObject = null;
        //}
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

    public void StartDictation()
    {
        StopAllCoroutines();
        debugWindow.Clear();

        dictationSubsystem.StartDictation();
        speechOutput.OnDictation();
        //dictationSubsystem.Start();

        //HardCodedPrompt();
    }

    private void HardCodedPrompt()
    {
        StartCoroutine(objectHighlighter.ClearAllHighlights());
        var hardcodedPrompt = "Where should the CPU be placed on the Motherboard?";
        dictationResult = hardcodedPrompt;
        promptAnswerText.text = "Sending prompt: '" + hardcodedPrompt + "'";
        imageCapture.CaptureImageAndSendIt();
    }
}
