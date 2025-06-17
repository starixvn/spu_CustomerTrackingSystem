namespace Fanex.NAP.CTS.Model.AIInsights
{
    using System.Collections.Generic;

    public class AIInsightsChatbotResponseModel
    {
        public AIInsightsChatbotDataModel response { get; set; }

        public List<ChatMessageModel> ChatMessages { get; set; }
    }
}