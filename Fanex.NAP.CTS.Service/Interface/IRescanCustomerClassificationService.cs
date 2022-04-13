namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface IRescanCustomerClassificationService
    {
        void RescanCustomerClassify(RescanCustClassificationModel para);

        void CheckNewPAAssociationCustomerForRollback(IEnumerable<CheckPACustomerAssociationForRollbackModel> assCustForRollbacks);

        IEnumerable<CustEvidenceInsertModel> GetProblemAccountLosing(IEnumerable<CustEvidenceModel> custEvidences, FunctionId functionId);

        IEnumerable<InsertProblemAccountModel> GetProblemAccountLosing(IEnumerable<MultipleProblemAccountsModel> problemAccounts, FunctionId functionId);
    }
}
