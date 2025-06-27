namespace Fanex.NAP.CTS.Model.AIInsights
{
    using System;

    [Serializable]
    public class ChatMessageModel
    {
        public string Sender { get; set; } // "User" or "Bot"

        public string Message { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
