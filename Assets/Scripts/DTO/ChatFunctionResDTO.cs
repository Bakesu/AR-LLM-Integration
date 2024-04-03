using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatFunctionResDTO : MonoBehaviour
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Choice
    {
        public int index { get; set; }
        public Message message { get; set; }
        public object logprobs { get; set; }
        public string finish_reason { get; set; }
    }

    public class Function
    {
        public string name { get; set; }
        public string arguments { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public object content { get; set; }
        public List<ToolCall> tool_calls { get; set; }
    }

    public class Root
    {
        public string id { get; set; }
        public string @object { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public List<Choice> choices { get; set; }
        public Usage usage { get; set; }
    }

    public class ToolCall
    {
        public string id { get; set; }
        public string type { get; set; }
        public Function function { get; set; }
    }

    public class Usage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }
 
}
