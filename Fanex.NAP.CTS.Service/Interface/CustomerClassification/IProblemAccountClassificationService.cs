namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification.ProblemAccount;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface IProblemAccountClassificationService
    {
        IEnumerable<long> RescanCustomerClassification(IEnumerable<RescanCustomerClassificationModel> custRescans, CCInputFlow ccInputFlow, bool isToQueue);

        DBResult InsertProblemAccountPackage(IEnumerable<InsertProblemAccountModel> problemAccounts, CCInputFlow ccInputFlow, string serviceName, FromFlow fromFlow, bool isToQueue);
                 
        IEnumerable<ProblemAccountLosingClassificationModel> GetProblemAccountLosingClassification(IEnumerable<long> custIds, FunctionId funcId);        
        
        void UpdateBetLimitOfFlaggedCustomer(IEnumerable<long> custId);

        void UpdateBetLimitOfUnflaggedCustomer(IEnumerable<long> custId);

        ResultRemoveCustomerClassificationModel UnmarkProblemAccount(IEnumerable<UnmarkProblemAccountModel> paUnmark, bool isFromQueue);        
    }
}
