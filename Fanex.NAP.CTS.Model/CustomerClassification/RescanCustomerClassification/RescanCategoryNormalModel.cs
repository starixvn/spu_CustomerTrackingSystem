namespace Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;

    [XName("r")]
    public class RescanCategoryNormalModel
    {
        [XName("CustId")]
        public long CustID { get; set; }
       
        [XName("CategoryId")]
        public int CategoryID { get; set; }
    }
}
