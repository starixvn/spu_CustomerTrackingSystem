namespace Fanex.NAP.CTS.Model.API
{
    using Newtonsoft.Json;
    using System.Runtime.Serialization;

    public class GetB2CustIDModel
    {
        [IgnoreDataMember]
        public int CustId { get; set; }

        [JsonProperty(PropertyName = "AccountID")]
        public string B2CCustId { get; set; }
    }
}