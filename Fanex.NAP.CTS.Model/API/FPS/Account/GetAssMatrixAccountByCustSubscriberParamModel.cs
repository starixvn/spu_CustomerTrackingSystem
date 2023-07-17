namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    public class GetAssMatrixAccountByCustSubscriberParamModel
    {
        public int SubscriberID { get; set; }

        public long CTSCustID { get; set; }

        public int NoOfCustomer { get; set; }

        public int NoOfDevice { get; set; }
    }
}
