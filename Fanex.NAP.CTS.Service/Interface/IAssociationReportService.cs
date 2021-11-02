namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.AssociationReport;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.ViewModel.AssociationReport;
    using System.Collections.Generic;

    public interface IAssociationReportService
    {
        #region Query From MySQL

        List<CustomerStaticStatusModel> GetCustomerStaticStatus(int listId, bool status);

        IEnumerable<AccountStaticStatusModel> GetAccountStaticStatuses();

        IEnumerable<EvidenceModel> GetEvidenceList();

        IEnumerable<CategoriesModel> GetCategories(string ctsCustIdList);

        GraphAssociationDetailModel GetAssociationDetail(string apiEndPoint, AssociationDetailFilterViewModel filter);

        DBResult ActiveRemoveAssociation(int fromSubscriberId, long fromCTSCustId, int toSubscriberId, long toCTSCustId, int createdBy, bool isRemoveAction);

        IEnumerable<AssociationFromMySqlModel> GetAssociationReportFromMySql(AssociationFilterViewModel filter, out int totalItems);

        IEnumerable<AssociationDetailFromMySqlModel> GetAssociationDetailFromMySql(AssociationDetailFilterViewModel filter, out int totalItems);

        #endregion Query From MySQL

        #region Query From GraphDB

        AssociationReportViewModel GetAssociationReportFromGraphDB(string apiEndPoint, AssociationFilterViewModel filter);

        AssociationReportViewModel MergeAssociationReport(AssociationReportViewModel custInfo, AssociationReportViewModel custAss, AssociationReportParameterModel para);

        #endregion Query From GraphDB
    }
}