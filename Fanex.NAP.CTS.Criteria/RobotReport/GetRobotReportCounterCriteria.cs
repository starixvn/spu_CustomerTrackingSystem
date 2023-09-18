namespace Fanex.NAP.CTS.Criteria.RobotReport
{
    using Fanex.Data.Repository;
    using System;

    public class GetRobotReportCounterCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int IsLicensee { get; set; }

        public override string GetSettingKey() => "CTS_Customer_RobotListSelByDate";

        public override bool IsValid() => true;
    }
}
