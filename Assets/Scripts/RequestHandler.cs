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
using System.Linq;
using System.Runtime.InteropServices;

public class RequestHandler : MonoBehaviour, MessageInterface
{

    [SerializeField]
    TextMeshProUGUI textMesh;

    [SerializeField]
    GameObject promptButton;

    [SerializeField]
    private ObjectHighlighter objectHighlighter;

    [SerializeField]
    private AIController aiController;

    private float temperature = 0.5f;
    private static object[] tools;

    private DebugWindow debugWindow;

    private List<MessageInterface> messageList = new List<MessageInterface>();

    private RequestDTO chatAndImageReqDTO;

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
        CreateTools();
        StartCoroutine(SetupGPT());
    }

    private IEnumerator SetupGPT()
    {
        yield return new WaitForSeconds(1);
        string sceneComponentList = CreateComponentList();

        string labelSystemPrompt =
        "You will be asked to help identify, locate or describe objects in provided images by using labels on the image, which will be detailed further now." +
        "\r\n The image will contain labels in a 8x5 grid ranging from A1 to E8." +
        "Each row begins with a letter. These letters, from top to bottom, range from 'A' to 'E' in alphabetical order." +
        "Additionally, each column ends with a number. These numbers, from left to right, range from '1' to '8' in numerical order." +
        "The labels are written in bold red letters and numbers and encased in a blue square." +
        "\r\n If you consider the requested area as clipping between multiple labels or covers multiple labels please provide all those labels" +
        "\r\n Your answer should be twofold." +
        "\r\n For the first section, please begin your answer with the label(s) of the grid cell " +
        "and wrap the label(s) in curly brackets. If there are multiple labels, insert a comma between each label." +
        "For the second section, after the curly brackets, " +

        "please answer the questions using a maximum of 30 words and without mentioning the grid or labels.";

        string functionSystemPrompt = @"The next line in square brackets is to be interpreted as a dictionary containing keys and values.
        \r\n[Motherboard:x1, CPU:x2, Ram:x3]\r\nIf the users asks about the location of a key in the dictionary, you are to highlight 
        the value corresponding to the inquiry. \r\nIf the user asks about how to place a component in the motherboard, you are to 
        request an image of the motherboard. \r\nIf the user doesnt ask about specific key, you are to provide a standard textual 
        answer which you give by calling the textual_answer function with the answer in the parameter. \r\nIf the user asks about 
        something in their environment that the assistant dont have the contextual knowledge about, \r\nyou are to capture an image to 
        provide context for the assistant.\r\nDon't make assumptions about what values to plug into functions. Ask for clarification if 
        a user request is ambiguous.";
        messageList.Add(new ReqMessage("system", new List<IContent> { new TextContent(functionSystemPrompt) }));

        Debug.Log(sceneComponentList);
    }

    //Creates component list based on the children of the objectHighlighter Gameobject
    private string CreateComponentList()
    {
        var sceneObjectList = "[";
        foreach (var qrObjectPair in objectHighlighter.imageTargets)
        {
            string listAppend = qrObjectPair.Key + ":" + qrObjectPair.Value.gameObject.name + ", ";
            sceneObjectList = string.Concat(sceneObjectList, listAppend);
        }
        char[] charsToTrim = { ',', ' ' };
        sceneObjectList = sceneObjectList.TrimEnd(charsToTrim);
        sceneObjectList = string.Concat(sceneObjectList, "]");
        return sceneObjectList;
    }

    internal void CreateFunctionCallRequest(string textPrompt)
    {
        byte[] requestBody = CreateFunctionCallRequestBody(textPrompt);
        StartCoroutine(CreateGPTRequest(requestBody));
    }

    internal void CreateImageRequest(string textPrompt, byte[] imageAsBytes)
    {
        byte[] requestBody = CreateImageRequestBody(textPrompt, imageAsBytes);
        StartCoroutine(CreateGPTRequest(requestBody));
    }

    internal IEnumerator CreateGPTRequest(byte[] requestBody)
    {
        var uwr = new UnityWebRequest(chatGptChatCompletionsUrl, "POST");
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(requestBody);
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
            ChatResDTO resultAsObject = JsonConvert.DeserializeObject<ChatResDTO>(result);
            ExtractedData extractedData = DataUtility.extractDataFromResponse(resultAsObject.choices[0].message.content);
            Debug.Log(resultAsObject.choices[0].message.tool_calls[0].function.name);
            Debug.Log(resultAsObject.choices[0].message.tool_calls[0].function.arguments);
            if (resultAsObject.choices[0].message.tool_calls.Count > 0)
            {
                var callList = resultAsObject.choices[0].message.tool_calls;
                foreach (var tool_call in callList)
                {
                    
                    aiController.GetType().GetMethod(tool_call.function.name).Invoke(aiController, new object[] { tool_call.function.arguments });
                }
            }
            messageList.Add(new Message("assistant", extractedData.TextContent));

            objectHighlighter.HighlightLabels(extractedData.Label);
            try
            {
                objectHighlighter.HighlightLabels(extractedData.Label);
                Debug.Log("label: " + String.Join(", ", extractedData.Label) + " + TextContent: " + extractedData.TextContent);
                textMesh.text = extractedData.TextContent;
            }
            catch (Exception e)
            {
                Debug.Log("No labels were given " + e);
            }

        }
    }

    static object AvailableFunctions(string name)
    {
        //callableFunctions.Add("highlight_objects", highlighted_object);

        return null;
    }

    private byte[] CreateImageRequestBody(string textPrompt, byte[] imageAsBytes)
    {
        string imageAsBase64 = "data:image/png;base64," + Convert.ToBase64String(imageAsBytes);
        var contentList = new List<IContent>
        {
            new TextContent(textPrompt),
            new ImageContent(imageAsBase64, "low")
        };
        //TODO: figure out what to do with image requests in messageList - image requests would break requests from the other type of body
        messageList.Add(new ReqMessage("user", contentList));
        chatAndImageReqDTO = new RequestDTO("gpt-4-vision-preview", 50, temperature, messageList);
        var requestBodyAsJSONString = JsonConvert.SerializeObject(chatAndImageReqDTO);
        return new System.Text.UTF8Encoding().GetBytes(requestBodyAsJSONString);
    }

    private byte[] CreateFunctionCallRequestBody(string textPrompt)
    {
        var contentList = new List<IContent>
        {
            new TextContent(textPrompt)
        };
        messageList.Add(new ReqMessage("user", contentList));
        chatAndImageReqDTO = new RequestDTO("gpt-4-turbo-preview", 50, temperature, messageList, tools);
        var requestBodyAsJSONString = JsonConvert.SerializeObject(chatAndImageReqDTO);
        Debug.Log(requestBodyAsJSONString);
        return new System.Text.UTF8Encoding().GetBytes(requestBodyAsJSONString);
    }

    static object[] CreateTools()
    {
        tools = new object[]
        {
            new {
                type = "function",
                function = new
                {
                    name = "highlight_objects",
                    description =  "Highlight the objects that the user mentions in their prompt",
                    parameters = new {
                        type = "object",
                        properties = new {
                            highlighted_object = new {
                            type = "string",
                            objectList = new [] {
                                "x1",
                                "x2",
                                "x3"
                            },
                            description = "The value corresponding to keys in the users prompt"
                    }
                    },
                        required = new[]
                        {
                            "highlighted_object"
                        }
                    }
                }
            },
            new {
                type = "function",
                function = new
                {
                    name = "capture_image",
                    description = "Capture an image from the point of view of the user to provide context for the assistant"
                }
            },
            new {
                type = "function",
                function = new
                {
                    name = "textual_answer",
                    description = "Provide a textual answer to the user's question",
                    parameters = new {
                        type = "object",
                        properties = new {
                            answer = new {
                                type = "string",
                                description = "The answer of the user's question"
                            }
                        },
                        required = new[] {
                            "answer"
                        }
                    }
                }
            },
            new {
                type = "function",
                function = new
                {
                    name = "get_label_image",
                    description = "When the user asks about a location on the motherboard, the assistant will request an image of the motherboard"
                }
            }
        };
        return tools;
    }

    public class ForceAcceptAll : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    //internal IEnumerator PromptRequest(string url, string json)
    //{

    //    var uwr = new UnityWebRequest(url, "POST");
    //    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
    //    uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
    //    uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    //    uwr.SetRequestHeader("Content-Type", "application/json");
    //    uwr.SetRequestHeader("Authorization", "Bearer " + APIKey);

    //    //Send the request then wait here until it returns
    //    yield return uwr.SendWebRequest();

    //    if (uwr.result == UnityWebRequest.Result.ConnectionError)
    //    {

    //        Debug.Log("Error While Sending: " + uwr.error);
    //    }
    //    else
    //    {
    //        var text = uwr.downloadHandler.text;
    //        ChatResDTO chatDTO = JsonConvert.DeserializeObject<ChatResDTO>(text);
    //        //messageList.Add(chatDTO.choices[0].message);
    //        textMesh.text = chatDTO.choices[0].message.content;
    //    }
    //}    
}

