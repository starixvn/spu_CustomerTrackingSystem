namespace Fanex.NAP.CTS.Model.SyncCustomerToMainDb
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;

    [XName("r")]
    public class CustomerClassForMainDb
    {
        [XName("id")]
        public int CustID { get; set; }

        [XName("c")]
        public int CustomerClass { get; set; }
    }
}
