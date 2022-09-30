namespace Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;

    [XName("r")]
    public class RescanCategoryDailyModel
    {
        [XName("CustId")]
        public long CustID { get; set; }
       
        [XName("CategoryId")]
        public int CurrentCategoryID { get; set; }
    }
}
