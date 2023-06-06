namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    public class GetAssAccountsByCustSubscriberParamModel
    {
        public int SubscriberID { get; set; }

        public long CTSCustID { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

    }
}
