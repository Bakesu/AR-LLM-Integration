using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Chat;
using ChatAndImage;
using MixedReality.Toolkit.Subsystems;

using MixedReality.Toolkit;

public class RequestHandler : MonoBehaviour, MessageInterface
{

    [SerializeField]
    TextMeshProUGUI textMesh;

    [SerializeField]
    GameObject promptButton;

    [SerializeField]
    private ObjectHighlighter objectHighlighter;

    private DebugWindow debugWindow;

    private List<MessageInterface> messageList = new List<MessageInterface>();

    private ChatAndImageReqDTO chatAndImageReqDTO;
    private int maximumContextLength = 2;
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

        string rulesPrompt =
            "You will be asked to help identify, locate or describe objects in provided images by using labels on the image, which will be detailed further now." +
            "\r\n The image will contain labels in a 8x5 grid ranging from A1 to E8." +
            "Each row begins with a letter. These letters, from top to bottom, range from 'A' to 'E' in alphabetical order." +
            "Additionally, each column ends with a number. These numbers, from left to right, range from '1' to '8' in numerical order." +
            "The labels are written in bold red letters and numbers and encased in a blue square." +
            //"\r\n If you consider an object as clipping between multiple labels please provide all those labels" +
            "\r\n Your answer should be twofold." +
            "\r\n For the first section, please begin your answer with the label of the grid cell " +
            "and wrap the label in curly brackets. For the second section, after the curly brackets, " +
            "please answer the questions using a maximum of 30 words and without mentioning the grid or labels.";
        messageList.Add(new ReqMessage("system", new List<IContent> { new TextContent(rulesPrompt) }));             

    }

    //internal string CreateAPIRequestBody(string textPrompt)
    //{
    //    var chatReqDTO = new ChatReqDTO();
    //    chatReqDTO.model = "gpt-3.5-turbo";
    //    chatReqDTO.temperature = 0.7;
    //    var message = new Message();
    //    message.role = "user";
    //    message.content = textPrompt;
    //    //messageList.Add(message);
    //    //chatReqDTO.messages = messageList;
    //    return JsonConvert.SerializeObject(chatReqDTO);
    //}

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
            string result = uwr.downloadHandler.text;
            Debug.Log(result);
            ChatAndImageResDTO resultAsObject = JsonConvert.DeserializeObject<ChatAndImageResDTO>(result);
            ExtractedData extractedData = Utility.extractDataFromResponse(resultAsObject.choices[0].message.content);
            messageList.Add(new Message("assistant", extractedData.TextContent));

            objectHighlighter.HighlightLabel(extractedData.Label);

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

        chatAndImageReqDTO = new ChatAndImageReqDTO("gpt-4-vision-preview", 50, messageList);
        var requestBodyAsJSONString = JsonConvert.SerializeObject(chatAndImageReqDTO);
        Debug.Log(requestBodyAsJSONString);
        return new System.Text.UTF8Encoding().GetBytes(requestBodyAsJSONString);
    }


public class ForceAcceptAll : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
}

