namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using Newtonsoft.Json;
    using System;

    public class MultipleProblemAccountsModel
    {
        public long CustID { get; set; }

        public int ReasonID { get; set; }

        //Default value = 0, except Check/Get RobotUser
        public int RobotCounter { get; set; }

        [JsonProperty("CreatedBy")]
        public Int64? Creator { get; set; }
    }
}