using System.Collections.Generic;

namespace Chat
{
    public class ChatReqDTO
    {
        public string model { get; set; }
        public List<Message> messages { get; set; }
        public double temperature { get; set; }
    }

}
