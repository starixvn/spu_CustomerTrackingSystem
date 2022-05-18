namespace Fanex.NAP.CTS.Model.CustomerClassify
{
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using System.Collections.Generic;
    using System.Linq;

    public class ProblemAccountInsertResultModel
    {
        public ProblemAccountInsertResultModel()
        {
            this.NewPAAssociationCustomerForRollback = Enumerable.Empty<CheckPACustomerAssociationForRollbackModel>();
        }

        public string ErrorMessage { get; set; }

        public int ErrorCode { get; set; }

        public IEnumerable<long> CustIds { get; set; }

        public IEnumerable<CheckPACustomerAssociationForRollbackModel> NewPAAssociationCustomerForRollback { get; set; }

    }
}
