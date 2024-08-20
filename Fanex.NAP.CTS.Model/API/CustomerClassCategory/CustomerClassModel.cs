namespace Fanex.NAP.CTS.Model.API
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;

    [XName("CustomerClass")]
    public class CustomerClassModel
    {
        public int CustId { get; set; }

        [XName("Class")]
        public int? CustomerClass { get; set; }
    }
}
