// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using System;
using System.Collections.Generic;

namespace ChatAndImage
{
    public class ChatAndImageReqDTO
    {
        public string model { get; set; }

        public int max_tokens { get; set; }
        public List<MessageInterface> messages { get; set; }

        public ChatAndImageReqDTO(string model, int max_tokens, List<MessageInterface> messages)
        {
            this.model = model;
            this.max_tokens = max_tokens;
            this.messages = messages;
        }
    }

    public class ReqMessage : MessageInterface
    {
        public string role { get; set; }
        public List<IContent> content { get; set; }

        public ReqMessage(string role, List<IContent> content)
        {
            this.role = role;
            this.content = content;
        }
    }

    public interface IContent
    {
        string type { get; set; }
    }

    public class ImageContent : IContent
    {
        public string type { get; set; }

        public ImageUrl image_url { get; set; }

        public ImageContent(string url, string detail)
        {
            this.type = "image_url";
            this.image_url = new ImageUrl(url, detail);
        }
    }

    public class ImageUrl
    {
        public string url { get; set; }
        public string detail { get; set; }
        public ImageUrl(string url, string detail)
        {
            this.url = url;
            this.detail = detail;
        }
    }

    public class TextContent : IContent
    {
        public string type { get; set; }
        public string text { get; set; }

        public TextContent(string text)
        {
            this.type = "text";
            this.text = text;
        }
    }

}

