namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.Account.AssociatedAccount;
    using Fanex.NAP.CTS.Model.Account.CustomerCategoryBySport;
    using Fanex.NAP.CTS.Model.Account.CustomerInfo;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model.Association;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.Home;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using System;
    using System.Collections.Generic;

    public interface IAccountService
    {        
        List<SearchResultModel> SearchAccounts(string username);

        #region Customer Profile
        CustomerInfoByCTSCustIdModel GetCustomerInfoByCTSCustId(long ctsCustId);

        CustomerInfoByCustIdsModel GetCustomerInfoByCustIds(long custId, int sourceType);

        CustomerInfoAssociationAccountModel GetCustomerInfoAssociationAccount(long ctsCustId, bool getFromMySQL);

        IEnumerable<CustomerInfoEvidenceModel> GetCustomerInfoEvidence(long ctsCustId);

        CustomerInfoLoginStatsModel GetCustomerInfoLoginStats(long ctsCustId, DateTime fromDate, DateTime toDate);

        CustomerInfoDateModel GetCustProfileDates(long ctsCustId);

        #endregion

        List<CustomerEvidenceByCTSCustIdModel> GetCustomerEvidenceByCTSCustId(long ctsCustId);

        List<CustomerStaticStatusModel> GetCustomerStaticStatus(int listId, bool status);

        CustomerProfileFraudInfoModel GetCustomerProfileFraudInfo(long custId);        

        List<TagModel> GetDirectAssociatedAccountTagFlagged(string custIdsAssInfo);

        CustomerLastTicketDateModel GetCustomerLastTicketDate(long custId);        

        CustomerRobotUserStatusModel GetCustomerRobotUserStatus(long custId);

        CustomerFlagInfoModel GetCustomerFlagInfo(long ctsCustId);

        //GraphAssociationPaging GetCustomerAssociationByGraph(string apiURL, long ctsCustId, int associationStatus, string accountStatusIds, string siteIds, int skipNumber, int takeNumber, bool fromMySQL = false);

        AssociatedAccountPagingModel GetAssociatedAccount(AssociatedFilterModel filterModel, bool fromMySQL = false);

        List<AccountStaticStatusModel> GetAccountStaticStatuses(int listId, bool status);

        List<GraphDirectAssociationModel> GetDirectAssocationByGraph(string apiURL, List<long> ctsCustIds);

        ShortestPathOfAccount GetShortestPathAssociationByGraph(string apiURL, int custIdFrom, int custIdTo);        

        string MappingAssociationType(AssociatedAccountModel data);

        IEnumerable<EvidenceModel> GetEvidenceList();

        IEnumerable<FlaggedEvidenceModel> GetFlaggedEvidences(long ctsCustId, string evidenceCodes);

        AffectedRetractEvidenceModel GetAffectedRetractEvidences(long ctsCustId, string evidenceCodes);

        DBResult RemoveFlaggedEvidence(RemoveEvidenceModel model);

        List<AccountExceptionsModel> GetAccountExceptions(long ctsCustId);

        DBResult AddAccountException(UpdateAccountExceptionModel model);

        int RemoveAccountException(UpdateAccountExceptionModel model);

        int RetractAffectedEvidence(RemoveEvidenceModel model);

        int RestoreAffectedEvidence(RemoveEvidenceModel model);

        IEnumerable<AffectedEvidenceListBetweenCustsModel> GetAffectedEvidenceListBetweenCusts(Int64 fromCTSCustID, Int64 toCTSCustID);

        IEnumerable<SportGroup> GetAllSportGroup();

        IEnumerable<ValidateManualAssociationMultipleModel> ValidateManualAssociationMultiple(long fromCTSCustId, string toUserNameList, out bool isException);

        DBResult InsertManualAssociationMultiple(string associationListJson);

        IEnumerable<CustomerCategory> GetCustomerCategoryByCTSCustId(long ctsCustId);

        DBResult ActiveRemoveAssociation(int fromSubscriberId, long fromCTSCustId, int toSubscriberId, long toCTSCustId, string remark, int createdBy, bool isRemoveAction);

        IEnumerable<long> GetAssociationException(string apiURL, long ctsCustId);

        IEnumerable<CustomerClassificationModel> GetCustomerClassification(long ctsCustId, out bool isPinCategory, out bool isExcludedTagAssPA);

        IEnumerable<CustomerClassificationModel> GetCustomerClassification(long ctsCustId);

        IEnumerable<CustomerClassificationHistoryModel> GetCustomerClassificationHistory(long ctsCustId);

        IEnumerable<LatestClassificationModel> MarkVVIPCategory(CustomerCategoryModel categoryModel);

        DBResult AddQueueAffectDownlinesCustomerCategory(CustomerCategoryModel categoryModel);

        IEnumerable<CustomerClassForMainDb> RemoveVVIPCategory(CustomerCategoryModel removeCategoryModel);        

        DBResult PinCategoryCustomerClassification(long ctsCustId, int subscriberId, bool isPinCategory, int createdBy);

        DBResult ChangeExcludedTagAssPA(long ctsCustId, int subscriberId, bool isPinCategory, int createdBy);

        IEnumerable<PerformanceBySportModel> GetPerformanceBySport(long custId, int getTop);

        IEnumerable<PerformanceByBettypeModel> GetPerformanceByBettype(long custId, int getTop);

        IEnumerable<PerformanceDailyModel> GetPerformanceDaily(long custId);

        IEnumerable<CustomerClassModel> GetCustomerClassByCustId(string custIdList);

        IEnumerable<CustomerClassificationModel> GetCustomerClassificationForAPI(string custIdList, string parentCateIdList);

        IEnumerable<CustomerClassificationGeneralInfoModel> GetCustomerClassificationByCategory(string categoryIdList);

        #region Customer Category By Sport
        IEnumerable<GetCustomerCategoryBySportModel> GetCustomerCategoryBySport(long ctsCustID);

        IEnumerable<GetCustomerCategoryHistoryBySportModel> GetCustomerCategoryHistoryBySport(long ctsCustID, int sportID);
        #endregion

        IEnumerable<CTSCustomer> CheckCustomerNotExisted(string custIds, DateTime fromDate, DateTime toDate);

        MatchesAssociationByAIModel GetGBAssociationFromAI(long custID1, long custID2, string groupList);

        IEnumerable<MatchesInfoModel> GetBettingPatternMatchesInfo(string matchIDs, string custIds);

        IEnumerable<MatchesModel> GetMatchInfosByIDs(string matchIDs);

        IEnumerable<CustomerGroupModel> GetCustGroupAssociation(string originalGroupIDs);

        IEnumerable<MatchesDetailInfoModel> GetTicketDetails(MatchesAssociationDetailViewModel macthInfo, string custIds, string betTypes);

        IEnumerable<MatchesDetailInfoViewModel> MergeCustGroupTicketInfo(IEnumerable<MatchCustomerGGBViewModel> custGroups, IEnumerable<MatchesDetailInfoModel> ticketDetails, bool isGGB);

        IEnumerable<MatchCustomerGGBViewModel> MergeCustMatchInfo(IEnumerable<GBAssociationAIModel> matchInfo, IEnumerable<CustomerGroupModel> groupInfo);

        IEnumerable<CategoryInfoModel> GetAllCategory();		

		AssociationByIPInfoModel GetAssociationByIPInfo(long custId1, long custId2);
        
    }
}