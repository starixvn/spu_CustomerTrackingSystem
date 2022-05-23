namespace Fanex.NAP.CTS.Model.LogInputData
{
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.DTO;
    using System.Collections.Generic;

    public class LogInputViewModel
    {
        public PlaceEvidencesViewModel PlaceEvidences { get; set; }

        public PlaceMultiCustomerEvidence PlaceMultiEvidences { get; set; }

        public List<InsertRobotModel> ProblemAccounts { get; set; }

        public IEnumerable<InsertRobotModel> RobotUser { get; set; }
    }
}
