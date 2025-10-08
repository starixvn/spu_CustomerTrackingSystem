namespace Fanex.NAP.CTS.ViewModel.AIInsights
{
    public class AIInsightEventSourceViewModel
    {
        public AIInsightEventSourceViewModel(AIInsightEventName eventName, AIInsightEventStatus eventStatus, object data)
        {
            EventName = eventName.ToString();
            EventStatus = eventStatus.ToString();
            Data = data;
        }

        public AIInsightEventSourceViewModel(AIInsightEventName eventName, AIInsightEventStatus eventStatus)
        {
            EventName = eventName.ToString();
            EventStatus = eventStatus.ToString();
        }


        public string EventName { get; set; }

        public string EventStatus { get; set; }

        public object Data { get; set; }
    }

    public enum AIInsightEventName
    {
        AnalyzeStart,
        AnalyzeSummary,
        AnalyzeCharts,
        AnalyzeComplete,
        Chatbot,
        Error,
    }

    public enum AIInsightEventStatus
    {
        Started,
        Processing,
        Completed,
        Failed,
    }
}
