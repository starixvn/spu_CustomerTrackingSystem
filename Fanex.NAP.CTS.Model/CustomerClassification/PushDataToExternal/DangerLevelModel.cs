namespace Fanex.NAP.CTS.Model.SyncCustomerToMainDb
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;

    [XName("r")]
    public class DangerLevelModel
    {
        [XName("id")]
        public long CustId { get; set; }

        [XName("l")]
        public int DangerLevel { get; set; }
    }
}