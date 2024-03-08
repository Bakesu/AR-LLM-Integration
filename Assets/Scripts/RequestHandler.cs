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

    [SerializeField]
    private ObjectHighlighter objectHighlighter;

    private DebugWindow debugWindow;

    private List<ReqMessage> messageList = new List<ReqMessage>();
     

    private byte[] imageToSend;
    private int maximumContextLength = 10;

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
        string promptEngineering =
            "You will be asked to identify, locate or describe objects in the provided image." +
            "\r\nThe image will contain a grid in blue (RGB value 0, 0, 255), " +
            "with each grid cell containing a label in red text (RGB value 255, 0, 0). " +
            "If an object is clipping between multiple grid cells, please provide " +
            "all the labels of all grid cells you deem to contain the object." +
            "\r\nYour answer should be twofold." +
            "\r\nFor the first section, please begin your answer with the label(s) of the grid cell(s) " +
            "and wrap the label(s) in curly brackets. For the second section, after the curly brackets, " +
            "please answer the questions using a maximum of 30 words and without mentioning the grid or labels." +
            "\r\n ''' " + textPrompt + "? '''";
        var requestBodyAsBytes = CreateImageRequestBody(promptEngineering, imageAsBytes);
        var uwr = new UnityWebRequest(chatGptChatCompletionsUrl, "POST");
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(requestBodyAsBytes);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.SetRequestHeader("Authorization", "Bearer " + APIKey);
        Debug.Log("Image saved to: " + Application.persistentDataPath);
        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {

            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            string result = uwr.downloadHandler.text;
            ChatAndImageResDTO resultAsObject = JsonConvert.DeserializeObject<ChatAndImageResDTO>(result);
            ExtractedData extractedData = Utility.extractDataFromResponse(resultAsObject.choices[0].message.content);
            //messageList.Add(promptAnswer);

            objectHighlighter.OnResponseReceived(extractedData);
            
            Debug.Log("label: " + extractedData.Label + ", TextContent: " + extractedData.TextContent);
            textMesh.text = extractedData.TextContent;
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

