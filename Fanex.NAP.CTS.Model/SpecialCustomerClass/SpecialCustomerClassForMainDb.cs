namespace Fanex.NAP.CTS.Model.SpecialCustomerClass
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;

    [XName("r")]
    public class SpecialCustomerClassForMainDb
    {
        [XName("id")]
        public int CustID { get; set; }

        [XName("c")]
        public int CustomerClass { get; set; }
    }
}
