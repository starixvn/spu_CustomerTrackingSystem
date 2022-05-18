namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using System;
    using System.Collections.Generic;

    public interface IProblemAccountClassificationService
    {
        ProblemAccountInsertResultModel InsertProblemAccountByCategory(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> probAccountLosing, int userID);

    }
}
