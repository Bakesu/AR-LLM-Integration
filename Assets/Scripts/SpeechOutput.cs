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
        var textToSpeechString = PlayerPrefs.GetString("texttospeech");
        if (textToSpeechString != "" || textToSpeechString != null)
        {
            textToSpeechSubsystem.TrySpeak(textToSpeechString, audioSource);
            promptAnswerText.text = textToSpeechString;
            PlayerPrefs.DeleteKey("texttospeech");
        }

    }

    public void ReloadSceneAndTextToSpeech(string recievedText)
    {
        PlayerPrefs.SetString("texttospeech", recievedText);
        VuforiaApplication.Instance.Initialize();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnSpeak(string text)
    {
        textToSpeechSubsystem.TrySpeak(text, audioSource);
        promptAnswerText.text = text;
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
