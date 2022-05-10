namespace Fanex.NAP.CTS.Model.CustomerClassify
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using System;
    using System.Xml.Serialization;

    [XName("r")]
    public class LatestClassification4ReportModel
    {
        [XName("id")]
        public int CustId { get; set; }

        [XName("c")]
        public int CustomerClass { get; set; }

        [XName("n")]
        public string CustomerClassName { get; set; }

        [XName("t")]
        public int Category { get; set; }

        [XmlIgnore]
        public DateTime ScannedTime { get; set; }

        public int? CustClass
        {
            get
            {
                if (CustomerClass == -1 || CustomerClass == -9)
                {
                    return null;
                }

                return CustomerClass;
            }
        }
    }
}