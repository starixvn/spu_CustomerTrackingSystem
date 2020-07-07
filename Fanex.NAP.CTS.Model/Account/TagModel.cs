namespace Fanex.NAP.CTS.Model.Account
{
    public class TagModel
    {
        public int SubscriberID { get; set; }

        public long CTSCustID { get; set; }

        public bool IsFlagged { get; set; }        

        public bool IsAffected { get; set; }
    }
}
