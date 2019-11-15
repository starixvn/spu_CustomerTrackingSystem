namespace Fanex.NAP.CTS.Model.Subscriber
{
    public class SubscriberAssignmentUpdate
    {
        public bool IsMasterSub { get; set; }

        public int UserId { get; set; }

        public string SubscriberIds { get; set; }

        public int CreatedBy { get; set; }
    }
}