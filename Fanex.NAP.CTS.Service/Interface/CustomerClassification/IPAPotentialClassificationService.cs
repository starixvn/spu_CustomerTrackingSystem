namespace Fanex.NAP.CTS.Service.Interface.CustomerClassification
{
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification;
    using System.Collections.Generic;

    public interface IPAPotentialClassificationService
    {
        IEnumerable<RescanPADangerousModel> GetDangerousScorePA(string custIDs);

        IEnumerable<TWInitialSmartPAModel> GetInitialSmartPA(string custIDs);

        IEnumerable<GetInitialTWGroupBettingModel> GetInitialTWGroupBetting(string custIDs);

        ResultInsertCustomerClassificationModel InsertDangerousScorePA(string custIDs);

        ResultInsertCustomerClassificationModel InsertInitialSmartPA(string custIDs);

        ResultInsertCustomerClassificationModel InsertInitialTWGroupBettingPA(string custIDs);

    }
}
