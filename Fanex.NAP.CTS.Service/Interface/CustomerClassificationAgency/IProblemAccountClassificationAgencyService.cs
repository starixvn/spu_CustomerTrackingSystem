namespace Fanex.NAP.CTS.Service.Interface.CustomerClassificationAgency
{
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.ProblemAccountAgency;
    using System.Collections.Generic;

    public interface IProblemAccountClassificationAgencyService
    {
        ResultRemoveCustomerClassificationAgencyModel UnmarkProblemAccountAgency(IEnumerable<UnmarkProblemAccountAgencyModel> paUnmark, bool isFromQueue);
    }
}
