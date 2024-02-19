using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using System.Buffers.Text;
using Chat;
using ChatAndImage;
using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit;

public class RequestHandler : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI textMesh;

    [SerializeField]
    GameObject promptButton;

    private DebugWindow debugWindow;

    private List<ReqMessage> messageList = new List<ReqMessage>();

    private byte[] imageToSend;

    internal string chatGptChatCompletionsUrl = "https://api.openai.com/v1/chat/completions";
    internal string APIKey = "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";


    public void Start()
    {
        ServicePointManager.ServerCertificateValidationCallback = delegate (
        object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors sslPolicyErrors
        )
        {
            return true; // Always accept
        };
    }
    internal string CreateAPIRequestBody(string textPrompt)
    {
        var chatReqDTO = new ChatReqDTO();
        chatReqDTO.model = "gpt-3.5-turbo";
        chatReqDTO.temperature = 0.7;
        var message = new Message();
        message.role = "user";
        message.content = textPrompt;
        //messageList.Add(message);
        //chatReqDTO.messages = messageList;
        return JsonConvert.SerializeObject(chatReqDTO);
    }

    internal IEnumerator PromptRequest(string url, string json)
    {

        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.SetRequestHeader("Authorization", "Bearer " + APIKey);

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {

            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            var text = uwr.downloadHandler.text;
            ChatResDTO chatDTO = JsonConvert.DeserializeObject<ChatResDTO>(text);
            //messageList.Add(chatDTO.choices[0].message);
            textMesh.text = chatDTO.choices[0].message.content;
        }
    }

    internal IEnumerator ImageRequest(string textPrompt, byte[] imageAsBytes)
    {
        var requestBodyAsBytes = CreateImageRequestBody(textPrompt, imageAsBytes);
        var uwr = new UnityWebRequest(chatGptChatCompletionsUrl, "POST");
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(requestBodyAsBytes);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.SetRequestHeader("Authorization", "Bearer " + APIKey);

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {

            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log(uwr.result);
            Debug.Log(uwr.downloadHandler.text);
            string result = uwr.downloadHandler.text;
            ChatAndImageResDTO resultAsObject = JsonConvert.DeserializeObject<ChatAndImageResDTO>(result);
            Message promptAnswer = resultAsObject.choices[0].message;
            //messageList.Add(promptAnswer);
            textMesh.text = promptAnswer.content;
        }
    }
    private byte[] CreateImageRequestBody(string textPrompt, byte[] imageAsBytes)
    {
        string imageAsBase64 = "data:image/png;base64," + Convert.ToBase64String(imageAsBytes);
        var contentList = new List<IContent>
        {
            new TextContent(textPrompt),
            new ImageContent(imageAsBase64, "low")
        };

        messageList.Add(new ReqMessage("user", contentList));

        //var messageList = new List<ReqMessage>
        //{
        //    new ReqMessage("user", contentList)
        //};

        var chatAndImageReqDTO = new ChatAndImageReqDTO("gpt-4-vision-preview", 50, messageList);
        var requestBodyAsJSONString = JsonConvert.SerializeObject(chatAndImageReqDTO);
        Debug.Log(requestBodyAsJSONString);
        return new System.Text.UTF8Encoding().GetBytes(requestBodyAsJSONString);
    }
}

public class ForceAcceptAll : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}

