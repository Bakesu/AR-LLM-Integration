using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit;
using TMPro;
using UnityEngine.SceneManagement;
using Vuforia;
using System.Threading.Tasks;
using System;

public class SpeechOutput : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    TextToSpeechSubsystem textToSpeechSubsystem;

    [SerializeField]
    TextMeshProUGUI promptAnswerText;

    string systemResponse = "I'm listening";

    //private Task<bool> isSpeaking;

    // Start is called before the first frame update
    private void Start()
    {
        textToSpeechSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<TextToSpeechSubsystem>();
        if (textToSpeechSubsystem == null)
        {
            Debug.LogError("No TextToSpeechSubsystem found");
        }
    }

    public void TextToSpeech(string recievedText)
    {
        Debug.Log("kaldt");
        textToSpeechSubsystem.TrySpeak(recievedText, audioSource);
        promptAnswerText.text = recievedText;
        
    }

    public void StopCurrentSentence()
    {
        textToSpeechSubsystem.TrySpeak(string.Empty, audioSource);
    }

    public void OnDictation()
    {
        textToSpeechSubsystem.TrySpeak(systemResponse, audioSource);
        promptAnswerText.text = systemResponse;

    }
}
