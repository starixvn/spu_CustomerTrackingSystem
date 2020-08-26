namespace Fanex.NAP.CTS.Model.Account
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;

    [XName("Row")]
    public class CustomerAssociationModel
    {
        [XName("RootCustID")]
        public int RootCustId { get; set; }

        [XName("CustID")]
        public int AffectedCustId { get; set; }
    }
}
