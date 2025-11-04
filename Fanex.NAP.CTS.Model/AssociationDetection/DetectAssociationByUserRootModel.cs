namespace Fanex.NAP.CTS.Model.AssociationDetection
{
    using Newtonsoft.Json;

    public class DetectAssociationByUserRootModel
    {      
        public long RootCustID { get; set; }

        [JsonProperty(PropertyName = "CustIDAssociations")]
        public string AssCustIDList { get; set; }
    }
}
