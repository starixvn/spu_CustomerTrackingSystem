namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.CTS.Model.CustomerClassification;    
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface IProblemAccountClassificationService
    {
        DBResult InsertProblemAccountPackage(IEnumerable<InsertProblemAccountModel> problemAccounts, InsertPA_ByType mappingCategoryByType, string serviceName, FromFlow fromFlow);

        InsertPAResultModel InsertProblemAccountLogics(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> paLosing, InsertPA_ByType mappingCategoryByType, string serviceName);

        IEnumerable<ExistedProblemAccountModel> GetExistedProblemAccounts(IEnumerable<long> custIds);

        IEnumerable<ProblemAccountLosingClassificationModel> GetProblemAccountLosingClassification(IEnumerable<long> custIds, FunctionId funcId);

        IEnumerable<InsertRobotModel> GetExistedTWRobotUser(IEnumerable<long> custIds);

        IEnumerable<CustInfoModel> InsertRobotAccount(IEnumerable<InsertRobotModel> robotAccounts);
    }
}
