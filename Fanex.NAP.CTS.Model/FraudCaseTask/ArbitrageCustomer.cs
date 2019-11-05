namespace Fanex.NAP.CTS.Model.FraudCaseTask
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;

    [XName("Association")]
    public class ArbitrageCustomer
    {
        public int CustId { get; set; }

        public int GroupId { get; set; }
    }
}