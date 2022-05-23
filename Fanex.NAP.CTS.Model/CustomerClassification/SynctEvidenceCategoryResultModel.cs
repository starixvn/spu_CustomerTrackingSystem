namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using System.Collections.Generic;

    public class SynctEvidenceCategoryResultModel
    {
        public int ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public IEnumerable<CheckPACustomerAssociationForRollbackModel> CustInfo { get; set; }
    }
}