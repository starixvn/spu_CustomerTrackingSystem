namespace Fanex.NAP.CTS.Model.Account
{
    public class ValidateManualAssociationMultipleModel
    {
        public string UserName { get; set; }

        public long CTSCustID { get; set; }

        public int SubscriberID { get; set; }

        public bool IsDuplicated { get; set; }

    }
}
