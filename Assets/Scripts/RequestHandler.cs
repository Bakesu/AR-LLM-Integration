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
using MixedReality.Toolkit.Subsystems;
using ChatAndImage;

using MixedReality.Toolkit;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using Tool = Chat.Tool;

public class RequestHandler : MonoBehaviour, MessageInterface
{

    [SerializeField]
    TextMeshProUGUI promptAnswerText;

    [SerializeField]
    GameObject promptButton;

    [SerializeField]
    private ObjectHighlighter objectHighlighter;

    [SerializeField]
    private FunctionCallHandler functionCallHandler;

    private float temperature = 0.1f;
    private static object[] tools;

    private DebugWindow debugWindow;

    private List<MessageInterface> messageList = new List<MessageInterface>();

    private RequestDTO chatAndImageReqDTO;

    internal string chatGptChatCompletionsUrl = "https://api.openai.com/v1/chat/completions";
    internal string APIKey = "api-key";

    string defaultTextSystemPrompt;
    string labelSystemPrompt;
    string functionSystemPrompt;
    internal bool imageRequestDone;

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

        defaultTextSystemPrompt = @"You will be asked to help identify, locate, describe objects or give general information from a provided image. 
        Only answer with 40 words";

        labelSystemPrompt = @"You will be asked to help identify, locate or describe objects in provided images by using labels on the image, 
        which will be detailed further now. The image will contain labels in a 8x5 grid ranging from A1 to E8. Each row begins with a letter. 
        These letters, from top to bottom, range from 'A' to 'E' in alphabetical order. Additionally, each column ends with a number. 
        These numbers, from left to right, range from '1' to '8' in numerical order. The labels are written in bold red letters and numbers 
        and encased in a blue square. If you consider the requested area as clipping between multiple labels or covers multiple labels please
        provide all those labels. Your answer should be twofold. For the first section, please begin your answer with the label(s) of the grid cell
        and wrap the label(s) in curly brackets. If there are multiple labels, insert a comma between each label. For the second section, 
        after the curly brackets, please answer the questions using a maximum of 30 words and without mentioning the grid or labels.";

        functionSystemPrompt = @"The next line in square brackets is to be interpreted as a dictionary containing keys and values."
        + DataUtility.CreateComponentList(objectHighlighter.imageTargets) +
        @" When calling a function, you should ALWAYS return a key from the dictionary.
        If the users asks about the location of a key in the dictionary, you are to highlight the value corresponding to the inquiry. 
        If the user asks about how to place a component on the motherboard, you are to request an image of the motherboard. 
        If the user doesnt ask about specific key, you are to provide a standard textual answer which you give by calling the TextualAnswer 
        function with the answer in the parameter. 
        If the user asks about something in their environment that the assistant dont have the contextual knowledge about, 
        you are to capture an image to provide context for the assistant with the function CaptureImage.";
        //Don't make assumptions about what values to plug into functions. Ask for clarification if a user request is ambiguous.";

        messageList.Add(new ReqMessage("system", new List<IContent> { new TextContent(functionSystemPrompt) }));
        Debug.Log("Ready");        
    }

    internal void CreateFunctionCallRequest(string textPrompt)
    {
        byte[] requestBody = CreateFunctionCallRequestBody(textPrompt);
        StartCoroutine(CreateGPTRequest(requestBody));
    }

    internal void CreateImageRequest(string textPrompt, byte[] imageAsBytes, bool isWithLabels)
    {
        byte[] requestBody = CreateImageRequestBody(textPrompt, imageAsBytes, isWithLabels);
        StartCoroutine(CreateGPTRequest(requestBody));        
    }

    internal IEnumerator CreateRecursiveImageRequest(string textPrompt, byte[] imageAsBytes, bool isWithLabels)
    {
        byte[] requestBody = CreateImageRequestBody(textPrompt, imageAsBytes, isWithLabels);
        yield return CreateGPTRequest(requestBody);
    }

    private byte[] CreateImageRequestBody(string textPrompt, byte[] imageAsBytes, bool isWithLabels)
    {
        if(isWithLabels)
        {
            messageList[0] = new ReqMessage("system", new List<IContent> { new TextContent(labelSystemPrompt) });
        } else
        {
            messageList[0] = new ReqMessage("system", new List<IContent> { new TextContent(defaultTextSystemPrompt) });
        }

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
        Debug.Log(requestBodyAsJSONString);
        return new System.Text.UTF8Encoding().GetBytes(requestBodyAsJSONString);
    }

    private byte[] CreateFunctionCallRequestBody(string textPrompt)
    {
        messageList[0] = new ReqMessage("system", new List<IContent> { new TextContent(functionSystemPrompt) });

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
            HandleGPTResponse(resultAsObject);
        }
    }

    private void HandleGPTResponse(ChatResDTO result)
    {
        Message message = result.choices[0].message;
        string finishedReason = result.choices[0].finish_reason;
        if (finishedReason == "tool_calls" || message.content == null) //If the response is a function call
        {
            if (message.tool_calls.Count > 0)
            {
                List<Tool> callList = message.tool_calls;
                foreach (var toolCall in callList)
                {
                    functionCallHandler.GetType().GetMethod(toolCall.function.name).Invoke(functionCallHandler, new object[] { toolCall.function.arguments });
                }
            }
        }
        else //If the response is a vision response
        {
            bool isLabelResponse = DataUtility.IsLabelResponse(message.content);
            if (isLabelResponse)
            {
                ExtractedLabelData extractedLabelData = DataUtility.extractDataFromResponse(message.content);
                functionCallHandler.HighlightLabels(extractedLabelData);
                messageList.Add(new Message("assistant", extractedLabelData.TextContent));
            }
            else
            {
                promptAnswerText.text = message.content;
                messageList.Add(new Message("assistant", message.content));
            }

        }
    }

    static object[] CreateTools()
    {
        tools = new object[]
        {
            new {
                type = "function",
                function = new
                {
                    name = "HighlightObjects",
                    description =  "Highlight the objects that the user mentions in their prompt",
                    parameters = new {
                        type = "object",
                        properties = new {
                            componentName = new {
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
                            "componentName"
                        }
                    }
                }
            },
            new {
                type = "function",
                function = new
                {
                    name = "CaptureImage",
                    description = "Capture an image from the point of view of the user to provide context for the assistant"
                }
            },
            new {
                type = "function",
                function = new
                {
                    name = "TextualAnswer",
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
                    name = "GiveInstructions",
                    description = "When the user asks about a location on the motherboard, the assistant will request an image of the motherboard. Parameters should be contained in the objectList",
                    parameters = new {
                        type = "object",
                        properties = new
                        {
                            placeableObject = new
                            {
                                type = "string",
                                objectList = new []
                                {
                                    "x1",
                                    "x2",
                                    "x3"
                                },
                                description = "The value corresponding to the key that the user wants to know where to put or how to place."
                            },
                            assemblingObject = new
                            {
                                type = "string",
                                objectList = new []
                                {
                                    "x1",
                                    "x2",
                                    "x3"
                                },
                                description = "The value corresponding to the key that the user is assembling/constructing/building."
                            }
                        },
                        required = new[] {
                            "placeableObject",
                            "assemblingObject"
                        }
                    },
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
}

