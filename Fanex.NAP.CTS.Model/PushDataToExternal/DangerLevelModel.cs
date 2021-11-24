namespace Fanex.NAP.CTS.Model.SyncCustomerToMainDb
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using System;
    using System.Xml.Serialization;

    [XName("r")]
    public class DangerLevelModel
    {
        [XName("id")]
        public long CustId { get; set; }

        [XName("l")]
        public int DangerLevel { get; set; }

        [XmlIgnore]
        public DateTime ScannedTime { get; set; }
    }
}