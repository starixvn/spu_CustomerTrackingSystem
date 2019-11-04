namespace Fanex.NAP.CTS.Library.Models.FraudCaseTask
{
    using Fanex.NAP.CTS.Library.Core.Helpers.Xml;

    [XName("Association")]
    public class ArbitrageCustomer
    {
        public int CustId { get; set; }

        public int GroupId { get; set; }
    }
}