namespace Fanex.NAP.CTS.Model.Subscriber
{
    public class SubscriberAssignment
    {
        public int SubscriberId { get; set; }

        public string SubscriberName { get; set; }

        public int IsAssigned { get; set; }

        public int IsMaster { get; set; }
    }
}