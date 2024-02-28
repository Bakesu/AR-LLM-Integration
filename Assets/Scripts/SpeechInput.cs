using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class SpeechInput : MonoBehaviour
{
    DictationSubsystem dictationSubsystem;

    [SerializeField]
    DebugWindow debugWindow;

    [SerializeField]
    RequestHandler requestHandler;

    [SerializeField]
    GameObject promptButton;

    [SerializeField]
    Texture2D hardcodedImage;

    ImageCapture imageCapture;

    [SerializeField] GameObject imgInBackground;

    internal String dictationResult;

    private int i = 0;
    void Start()
    {
        imageCapture = GetComponentInParent<ImageCapture>();

        // Get the first running dictation subsystem.
        dictationSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<DictationSubsystem>();

        // If we found one...
        if (dictationSubsystem != null)
        {
            // Add event handlers to all dictation subsystem events. 
            dictationSubsystem.Recognizing += Dictation_Recognizing;
            dictationSubsystem.Recognized += Dictation_Recognized;
            //dictationSubsystem.RecognitionFinished += Dictation_RecognitionFinished;
            dictationSubsystem.RecognitionFaulted += Dictation_RecognitionFaulted;

            // And start dictation
            //dictationSubsystem.StartDictation();
        }
    }

    private void Dictation_RecognitionFaulted(DictationSessionEventArgs args)
    {
        Debug.Log(args.ReasonString);
        promptButton.GetComponent<ButtonDisabler>().EnableButton();
    }

    private void Dictation_RecognitionFinished(DictationSessionEventArgs args)
    {
        throw new NotImplementedException();
    }

    private void Dictation_Recognized(DictationResultEventArgs args)
    {
        debugWindow.Clear();
        dictationResult = args.Result;
        Debug.Log("Sending prompt:" + "'" + dictationResult + "'");
        dictationSubsystem.StopDictation();
        imageCapture.CaptureImageAndSendIt();
        //Flip the sprite on the x-axis
        //imgInBackground.GetComponent<SpriteRenderer>().flipX = !imgInBackground.GetComponent<SpriteRenderer>().flipX;
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
        //promptButton.GetComponent<ButtonDisabler>().DisableButton();
        dictationSubsystem.StartDictation();
        Debug.Log("Start Dictating!");
    }


}
