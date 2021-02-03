namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model;   
    using Fanex.NAP.CTS.Model.Association;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.Home;    
    using System;
    using System.Collections.Generic;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Model.CustomerClassify;

    public interface IAccountService
    {        
        List<SearchResultModel> SearchAccounts(string username);

        List<CustomerGeneralStatusModel> GetCustomerGeneralStatus(string custIds);

        CustomerInfoByCTSCustIdModel GetCustomerInfoByCTSCustId(long ctsCustId);

        CustomerAssociationByCTSCustIdModel GetCustomerTotalAssociationByCTSCustId(long ctsCustId, string apiUrl, bool fromMySQL = false);

        List<CustomerEvidenceByCTSCustIdModel> GetCustomerEvidenceByCTSCustId(long ctsCustId);

        List<CustomerStaticStatusModel> GetCustomerStaticStatus(int listId, bool status);

        CustomerProfileFraudInfoModel GetCustomerProfileFraudInfo(long custId);    

        CustomerPoliciesModel GetCustomerProfilePoliciesInfo(long custId);        

        List<TagModel> GetDirectAssociatedAccountTagFlagged(string custIdsAssInfo);

        CustomerLastTicketDateModel GetCustomerLastTicketDate(long custId);

        CustomerEsportAutoSuspendStatusModel GetCustomerEsportAutoSuspendedStatus(long custId);

        CustomerRobotUserStatusModel GetCustomerRobotUserStatus(long custId);

        CustomerFlagInfoModel GetCustomerFlagInfo(long ctsCustId);

        GraphAssociationPaging GetCustomerAssociationByGraph(string apiURL, long ctsCustId, int associationStatus, string accountStatusIds, string siteIds, int skipNumber, int takeNumber, bool fromMySQL = false);

        List<AccountStaticStatusModel> GetAccountStaticStatuses();

        IEnumerable<SubscriberGroupModel> GetAllSubscriberGroups();

        List<GraphDirectAssociationModel> GetDirectAssocationByGraph(string apiURL, List<long> ctsCustIds);

        ShortestPathOfAccount GetShortestPathAssociationByGraph(string apiURL, int custIdFrom, int custIdTo);        

        List<GraphAssociationModel> FilterCustomerAssociationByStatus(string statusIds, List<GraphAssociationModel>  customerAssociation, List<CustomerGeneralStatusModel> statusByCustIds);

        List<GraphAssociationModel> FilterCustomerAssociationByAssociationType(int associationType, List<GraphAssociationModel> customerAssociation);

        IEnumerable<EvidenceModel> GetEvidenceList();

        IEnumerable<FlaggedEvidenceModel> GetFlaggedEvidences(long ctsCustId, string evidenceCodes);

        AffectedRetractEvidenceModel GetAffectedRetractEvidences(long ctsCustId, string evidenceCodes);

        DBResult PlaceEvidence(UpdatePlaceEvidenceModel model);

        void FPSAddNewEvidence(string apiURL, UpdatePlaceEvidenceModel model);

        DBResult RemoveFlaggedEvidence(RemoveEvidenceModel model);

        DBResult SyncCustEvidenceToCategory(string custEvidencesJson, bool isRescan, out IEnumerable<int> custIds);

        List<AccountExceptionsModel> GetAccountExceptions(long ctsCustId);

        int AddAccountException(UpdateAccountExceptionModel model);

        int RemoveAccountException(UpdateAccountExceptionModel model);

        int RetractAffectedEvidence(RemoveEvidenceModel model);

        int RestoreAffectedEvidence(RemoveEvidenceModel model);

        IEnumerable<AffectedEvidenceListBetweenCustsModel> GetAffectedEvidenceListBetweenCusts(Int64 fromCTSCustID, Int64 toCTSCustID);

        IEnumerable<SportGroup> GetAllSportGroup();

        List<AssociationCustomerModel> SearchAssociatedCustomer(string username);        

        IEnumerable<ValidateManualAssociationMultipleModel> ValidateManualAssociationMultiple(long fromCTSCustId, string toUserNameList, out bool isException);

        DBResult InsertManualAssociationMultiple(string associationListJson);

        IEnumerable<CustomerCategory> GetCustomerCategoryByCTSCustId(long ctsCustId);

        DBResult ActiveRemoveAssociation(int fromSubscriberId, long fromCTSCustId, int toSubscriberId, long toCTSCustId, string remark, int createdBy, bool isRemoveAction);

        IEnumerable<long> GetAssociationException(string apiURL, long ctsCustId);

        IEnumerable<CustomerClassificationModel> GetCustomerClassification(long custId, out bool isPinCategory);

        IEnumerable<CustomerClassificationHistoryModel> GetCustomerClassificationHistory(long custId);

        DBResult AddCustomerCategory(AddCustomerCategoryModel addCategoryModel, int createdBy, out IEnumerable<LatestClassificationModel> latestClassifications);
               
        ResultInsertRemoveCustomerClassificationModel RemoveCustomerCategory(long custId, long ctsCustId, int categoryId, int createdBy);        

        DBResult PinCategoryCustomerClassification(int custId, long ctsCustId, int subscriberId, bool isPinCategory, int createdBy);

        IEnumerable<PerformanceBySportModel> GetPerformanceBySport(long custId, int getTop);

        IEnumerable<PerformanceByBettypeModel> GetPerformanceByBettype(long custId, int getTop);

        IEnumerable<PerformanceDailyModel> GetPerformanceDaily(long custId);

        IEnumerable<CustomerClassModel> GetCustomerClassByCustId(string custIdList);

        IEnumerable<CustomerClassificationModel> GetCustomerClassificationForAPI(string custIdList, string parentCateIdList);

        IEnumerable<CustomerClassificationGeneralInfoModel> GetCustomerClassificationByCategory(string categoryIdList);

        DBResult ProcessPlaceEvidence(UpdatePlaceEvidenceModel model);

        DBResult ProcessPlaceEvidenceByCustId(UpdatePlaceEvidenceModel model);

        IEnumerable<CTSCustomer> CheckCustomerNotExisted(string custIds, DateTime fromDate, DateTime toDate);
    }
}