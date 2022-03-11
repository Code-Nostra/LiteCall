using System;

namespace SignalRLibrary
{
    public class Message
    {
        public string Text { get; set; }
        public string Sender { get; set; }
        public DateTime DateSend { get; set; }

        public Message CreateMs(string _Sender,string _text, DateTime _DateSend)
        {
            return new Message
            {
                Text = _text,
                Sender = _Sender,
                DateSend = _DateSend
            };
        }
    }
}
