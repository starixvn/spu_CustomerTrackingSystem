namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using System.Collections.Generic;

    public interface IRescanCustomerClassificationService
    {
        bool RescanCustomerClassification(RescanCustClassificationModel parameters);

        IEnumerable<long> GetAgentCusts(IEnumerable<long> custIds);

        bool RescanNormalAccount(RescanCustClassificationModel parameters, IEnumerable<long> custIdsToRescanPA, IEnumerable<long> smaCustIds);

        IEnumerable<RescanCategoryNormalModel> GetExistingCustClassificationNormal(string custIdList);
    }
}
