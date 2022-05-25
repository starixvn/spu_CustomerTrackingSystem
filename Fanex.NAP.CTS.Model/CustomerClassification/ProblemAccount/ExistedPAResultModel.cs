namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using System.Collections.Generic;
    using System.Linq;

    public class ExistedPAResultModel
    {
        public ExistedPAResultModel()
        {
            ProblemAccounts = Enumerable.Empty<InsertProblemAccountModel>();
            RobotClassificationPerformance = Enumerable.Empty<InsertRobotModel>();
        }
        public IEnumerable<InsertProblemAccountModel> ProblemAccounts { get; set; }

        public IEnumerable<InsertRobotModel> RobotClassificationPerformance { get; set; }
    }
}