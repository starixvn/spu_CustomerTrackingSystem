namespace Fanex.NAP.CTS.Model.RobotReport
{
    using Newtonsoft.Json;
    using System;

    public class RobotReportCounterView
    {
        [JsonProperty("CustID")]
        public long custid { get; set; }

        //public int counter { get; set; }

        public DateTime RobotDate { get; set; }

        //public int IsLicensee { get; set; }
    }
}
