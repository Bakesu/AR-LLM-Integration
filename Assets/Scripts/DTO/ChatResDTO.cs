// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using ChatAndImage;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Chat
{

    [Serializable]
    public class ChatResDTO
    {
        public string id;
        public string @object;
        public int created;
        public string model;
        public List<Choice> choices;
        public Usage usage;
    }

    [Serializable]
    public class Choice
    {
        public int index;
        public Message message;
        public string finish_reason;
    }

    [Serializable]
    public class Message : MessageInterface
    {
        public string role;
        public string content;
        public List<Tool> tool_calls;

        public Message(string role, string content, [Optional] List<Tool> tool_calls)
        {
            this.role = role;
            this.content = content;
            this.tool_calls = tool_calls;
        }
    }
    public class Tool
    {
        public string id { get; set; }
        public string type { get; set; }
        public Function function { get; set; }
    }


    public class Function
    {
        public string name { get; set; }
        public string arguments { get; set; }
    }


    [Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }

}