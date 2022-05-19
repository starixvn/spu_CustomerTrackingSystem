namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using System;
    using System.Collections.Generic;

    public interface IProblemAccountClassificationService
    {
        bool InsertProblemAccount(IEnumerable<InsertProblemAccountModel> problemAccounts);

        void InsertProblemAccountByReason(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> probAccountLosing);

        ProblemAccountInsertResultModel InsertProblemAccountByCategory(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> probAccountLosing);
       
    }
}
