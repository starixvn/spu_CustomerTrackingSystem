namespace Fanex.NAP.CTS.Model.CustomerClassification.PushDataToExternal
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;

    [XName("r")]
    public class CustomerClassificationUpdateBySportToMainDBModel
    {
        [XName("id")]
        public long CustID { get; set; }

        [XName("s")]
        public int SportID { get; set; }

        [XName("c")]
        public int CustomerClass { get; set; }

    }
}
