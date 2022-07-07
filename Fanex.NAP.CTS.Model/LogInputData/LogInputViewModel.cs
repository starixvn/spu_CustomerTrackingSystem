namespace Fanex.NAP.CTS.Model.LogInputData
{
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using System.Collections.Generic;

    public class LogInputViewModel
    {
        public IEnumerable<InsertRobotModel> RobotUser { get; set; }

        public LicenseeVIPBALogDataModel LicenseeVIPBAModel { get; set; }

        public RemoveLicenseeBABySiteLogDataModel RemoveLicenseeBABySiteModel { get; set; }
    }
}
