namespace Fanex.NAP.CTS.Model.RobotReport
{
    using Fanex.NAP.CTS.Model.RobotReport.Entities;
    using Newtonsoft.Json;
    using System;

    public class RobotReportCounterModel
    {
        public RobotReportCounterModel() { }

        public RobotReportCounterModel(RobotReportCounterEntity entity)
        {
            custid = entity.custid;
            RobotDate = entity.RobotDate;
        }

        [JsonProperty("CustID")]
        public long custid { get; set; }

        //public int counter { get; set; }

        public DateTime RobotDate { get; set; }

        //public int IsLicensee { get; set; }
    }
}
