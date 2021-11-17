namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using System.Collections.Generic;

    public interface IRescanCustomerClassificationService
    {
        void RescanCustomerClassify(RescanCustClassificationModel para);

        int CheckNewPAAssociationCustomerForRollback(IEnumerable<NewPAAssociationCustomerForRollbackModel> assCustForRollbacks);
    }
}
