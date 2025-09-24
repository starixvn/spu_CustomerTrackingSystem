namespace Fanex.NAP.CTS.Model.AssociationDetection
{
    using Newtonsoft.Json;

    public class AssociationCustModel
    {
        [JsonProperty(PropertyName = "O")]
        public long FromCTSCustID { get; set; }
     
        [JsonProperty(PropertyName = "D")]
        public long ToCTSCustID { get; set; }       
    }
}
