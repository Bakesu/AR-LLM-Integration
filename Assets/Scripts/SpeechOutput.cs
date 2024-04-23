using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit;
using TMPro;

public class SpeechOutput : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    TextToSpeechSubsystem textToSpeechSubsystem;
    
    [SerializeField]
    TextMeshProUGUI promptAnswerText;

    string systemResponse;
    // Start is called before the first frame update
    private void Start()
    {
        textToSpeechSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<TextToSpeechSubsystem>();
        if (textToSpeechSubsystem == null)        
        {
            Debug.LogError("No TextToSpeechSubsystem found");
        }
    }

    public void TextToSpeech(string GPTResponse)
    {
        textToSpeechSubsystem.TrySpeak(GPTResponse, audioSource);
        promptAnswerText.text = GPTResponse;
    }

    public void StopCurrentSentence()
    {        
        textToSpeechSubsystem.TrySpeak(string.Empty, audioSource);
    }

    public void OnDictation()
    {        
        systemResponse = "I'm listening";
        textToSpeechSubsystem.TrySpeak(systemResponse, audioSource);
        promptAnswerText.text = systemResponse;

    }
}
