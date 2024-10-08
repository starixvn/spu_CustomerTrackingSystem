namespace Fanex.NAP.CTS.Service.Interface.CustomerClassificationAgency
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.ProblemAccountAgency;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface IProblemAccountClassificationAgencyService
    {
        ResultRemoveCustomerClassificationAgencyModel UnmarkProblemAccountAgency(IEnumerable<UnmarkProblemAccountAgencyModel> paUnmark, bool isFromQueue);
        
        DBResult InsertProblemAccountAgencyPackage(IEnumerable<InsertProblemAccountModel> problemAccounts, AgencyCCInputFlow ccInputFlow, string serviceName, FromFlow fromFlow, bool isToQueue);

    }
}
