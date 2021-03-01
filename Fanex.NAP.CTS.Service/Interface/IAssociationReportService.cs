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

        IEnumerable<SubscriberGroupModel> GetAllSubscriberGroups();

        GraphAssociationModel GetAssociations(string apiEndPoint, AssociationFilterViewModel filter);        

        IEnumerable<CategoriesModel> GetCategories(string ctsCustIdList);

        GraphAssociationDetailModel GetAssociationDetail(string apiEndPoint, AssociationDetailFilterViewModel filter);

        DBResult ActiveRemoveAssociation(int fromSubscriberId, long fromCTSCustId, int toSubscriberId, long toCTSCustId, int createdBy, bool isRemoveAction);

        IEnumerable<AssociationFromMySqlModel> GetAssociationFromMySql(AssociationFilterViewModel filter, out int totalItems);
    }
}
