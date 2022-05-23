namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using System;
    using System.Collections.Generic;

    public class ExistedPAResultModel
    {
        public IEnumerable<InsertProblemAccountModel> ProblemAccounts { get; set; }

        public IEnumerable<InsertRobotModel> RobotClassificationPerformance { get; set; }
    }
}