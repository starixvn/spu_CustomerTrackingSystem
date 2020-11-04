namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.AssociationReport;
    using Fanex.NAP.CTS.ViewModel.AssociationReport;
    using System.Collections.Generic;

    public interface IAssociationReportService
    {
        List<CustomerStaticStatusModel> GetCustomerStaticStatus(int listId, bool status);

        IEnumerable<AccountStaticStatusModel> GetAccountStaticStatuses();

        IEnumerable<EvidenceModel> GetEvidenceList();

        IEnumerable<AssociationModel> GetAssociations(string apiEndPoint, AssociationFilterViewModel filter);

        IEnumerable<CategoriesModel> GetCategories(string ctsCustIdList);

        IEnumerable<AssociationDetailModel> GetAssociationDetail(string apiEndPoint, AssociationDetailFilterViewModel filter);

        DBResult ActiveRemoveAssociation(int fromSubscriberId, long fromCTSCustId, int toSubscriberId, long toCTSCustId, int createdBy, bool isRemoveAction);
    }
}
