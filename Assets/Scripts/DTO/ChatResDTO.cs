// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using ChatAndImage;
using System;
using System.Collections.Generic;

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

        public Message(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }

    [Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }

}