namespace Fanex.NAP.CTS.Model.API
{
    using Newtonsoft.Json;

    public class CheckAccountExistModel
    {
        public string AccountID { get; set; }

        [JsonIgnore]
        public string Status { get; set; }
    }
}