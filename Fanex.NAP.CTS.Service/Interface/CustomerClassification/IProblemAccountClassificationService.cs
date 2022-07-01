namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification.ProblemAccount;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface IProblemAccountClassificationService
    {
        DBResult InsertProblemAccountPackage(IEnumerable<InsertProblemAccountModel> problemAccounts, InsertPA_ByType mappingCategoryByType, string serviceName, FromFlow fromFlow, bool isToQueue);

        IEnumerable<CustInfoModel> InsertProblemAccountLogics(IEnumerable<InsertProblemAccountModel> problemAccounts, FunctionId funcId, InsertPA_ByType mappingCategoryByType, bool isToQueue);

        void RemoveOldProblemAccount(IEnumerable<long> custIds);

        IEnumerable<ExistedProblemAccountModel> GetExistedProblemAccounts(IEnumerable<long> custIds);

        IEnumerable<ProblemAccountLosingClassificationModel> GetProblemAccountLosingClassification(IEnumerable<long> custIds, FunctionId funcId);

        IEnumerable<InsertRobotModel> GetExistedTWRobotUser(IEnumerable<long> custIds);

        IEnumerable<CustInfoModel> InsertRobotAccountPackge(IEnumerable<InsertRobotModel> robotAccounts, bool isRescan);

        void UpdateBetLimitOfFlaggedCustomer(IEnumerable<long> custId);

        void UpdateBetLimitOfUnflaggedCustomer(IEnumerable<long> custId);

        UnmarkPAResultModel UnmarkProblemAccount(IEnumerable<UnmarkProblemAccountModel> paUnmark, bool isFromQueue);        
    }
}
