namespace Fanex.NAP.CTS.Service.Interface.CustomerClassification
{
    using Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification;
    using System.Collections.Generic;

    public interface IPAPotentialClassificationService
    {
        IEnumerable<RescanPADangerousModel> GetDangerousScorePA(string custIDs);

        IEnumerable<TWInitialSmartPAModel> GetInitialSmartPA(string custIDs);

        IEnumerable<GetInitialTWGroupBettingModel> GetInitialTWGroupBetting(string custIDs);

    }
}
