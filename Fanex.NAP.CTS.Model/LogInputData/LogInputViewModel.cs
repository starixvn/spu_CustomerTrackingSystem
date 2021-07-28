namespace Fanex.NAP.CTS.Model.LogInputData
{
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.DTO;
    using System.Collections.Generic;

    public class LogInputViewModel
    {
        public PlaceEvidencesViewModel PlaceEvidences { get; set; }

        public PlaceMultiCustomerEvidence PlaceMultiEvidences { get; set; }

        public List<MultipleProblemAccountsModel> ProblemAccounts { get; set; }
    }
}
