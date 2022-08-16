namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    using Newtonsoft.Json;    

    public class SharedByMatchesInfoViewModel
    {
        [JsonProperty(PropertyName = "C")]
        public int CustID { get; set; }       

        [JsonProperty(PropertyName = "U")]
        public string UserName { get; set; }
    }
}
