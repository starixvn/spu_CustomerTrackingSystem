namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.Account.AssociatedAccount;
    using Fanex.NAP.CTS.Model.Account.CustomerCategoryBySport;
    using Fanex.NAP.CTS.Model.Account.CustomerInfo;
    using Fanex.NAP.CTS.Model.Association;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.Home;
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using System;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface IAccountService
    {        
        List<SearchResultModel> SearchAccounts(string username);

        #region Customer Profile

        IEnumerable<CustInfoModel> GetCustInfoByCustID(string custIDs);

        CustomerInfoByCtsCustIdModel GetCustomerInfoByCTSCustId(long ctsCustId);

        CustomerInfoByCustIdsModel GetCustomerInfoDangerous(long custID);

        CustomerInfoAssociationAccountModel GetCustomerInfoAssociationAccount(long ctsCustId);

        IEnumerable<CustomerInfoEvidenceModel> GetCustomerInfoEvidence(long ctsCustId);

        CustomerInfoLoginStatsModel GetCustomerInfoLoginStats(long ctsCustId, DateTime fromDate, DateTime toDate);

        CustomerInfoDateModel GetCustProfileDates(long ctsCustId, ROLE_CUSTOMER roleID);

        #endregion

        List<CustomerEvidenceByCTSCustIdModel> GetCustomerEvidenceByCTSCustId(long ctsCustId);

        List<CustomerStaticStatusModel> GetCustomerStaticStatus(int listId, bool status);      

        List<TagModel> GetDirectAssociatedAccountTagFlagged(string custIdsAssInfo);            

        CustomerFlagInfoModel GetCustomerFlagInfo(long ctsCustId);

        AssociatedAccountPagingModel GetAssociatedAccount(AssociatedFilterModel filterModel);

        List<AccountStaticStatusModel> GetAccountStaticStatuses(int listId, bool status);

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

        IEnumerable<ValidateManualAssociationMultipleModel> ValidateManualAssociationMultiple(long fromCTSCustId, string toUserNameList, out bool isException);

        DBResult InsertManualAssociationMultiple(string associationListJson);      

        DBResult ActiveRemoveAssociation(int fromSubscriberId, long fromCTSCustId, int toSubscriberId, long toCTSCustId, string remark, int createdBy, bool isRemoveAction);       

        IEnumerable<GetCustomerClassificationModel> GetCustomerClassification(long ctsCustId, ROLE_CUSTOMER Role, out bool isExcludedTagAssPA);

        IEnumerable<CustomerClassificationHistoryModel> GetCustomerClassificationHistory(long ctsCustId, ROLE_CUSTOMER RoleID);
      
        DBResult AddQueueAffectDownlinesCustomerCategory(CustomerCategoryModel categoryModel);       

        DBResult ChangeExcludedTagAssPA(long ctsCustId, int subscriberId, bool isPinCategory, int createdBy);

        IEnumerable<PerformanceBySportModel> GetPerformanceBySport(long custId, int getTop);

        IEnumerable<PerformanceByBettypeModel> GetPerformanceByBettype(long custId, int getTop);

        IEnumerable<PerformanceDailyModel> GetPerformanceDaily(long custId);
      
        IEnumerable<GetCustomerClassificationModel> GetCustomerClassificationForAPI(string custIdList, string parentCateIdList);

        IEnumerable<CustomerClassificationGeneralInfoModel> GetCustomerClassificationByCategory(string categoryIdList);

        #region Customer Category By Sport
        IEnumerable<GetCustomerCategoryBySportModel> GetCustomerCategoryBySport(long ctsCustID);

        IEnumerable<GetCustomerCategoryHistoryBySportModel> GetCustomerCategoryHistoryBySport(long ctsCustID, int sportID);
        #endregion

        MatchesAssociationByAIModel GetGBAssociationFromAI(long custID1, long custID2, string groupList);

        IEnumerable<MatchesInfoModel> GetBettingPatternMatchesInfo(string matchIDs, string custIds);

        IEnumerable<MatchesModel> GetMatchInfosByIDs(string matchIDs);

        IEnumerable<CustomerGroupModel> GetCustGroupAssociation(string originalGroupIDs);

        IEnumerable<MatchesDetailInfoModel> GetTicketDetails(MatchesAssociationDetailViewModel macthInfo, string custIds, string betTypes);

        IEnumerable<MatchesDetailInfoViewModel> MergeCustGroupTicketInfo(IEnumerable<MatchCustomerGgbViewModel> custGroups, IEnumerable<MatchesDetailInfoModel> ticketDetails, bool isGGB);

        IEnumerable<MatchCustomerGgbViewModel> MergeCustMatchInfo(IEnumerable<GBAssociationAIModel> matchInfo, IEnumerable<CustomerGroupModel> groupInfo);

        IEnumerable<CategoryInfoModel> GetAllCategory();		

		AssociationByIPInfoModel GetAssociationByIPInfo(long custId1, long custId2);

        IEnumerable<int> GetAssociationByIPType(long custID1, long custID2);

        IEnumerable<AssByIPTicketStakeModel> GetAssociationBy5GIPDetail(long custID1, long custID2);

        CoupleInfoViewModel GetCoupleInfos(long custID1, long custID2);

        IEnumerable<DateTime> GetAIFristAssociationDate(long custID1, long custID2, AccountAssociation_ByAIPattern pattern);

        IEnumerable<CoupleFraudDetailModel> GetCoupleFraudDetails(string coupleInfoDetails);

        IEnumerable<CoupleIPFraudDetailModel> GetCoupleIPFraudDetails(string coupleInfoDetails);

        IEnumerable<TicketDetailsModel> GetAIAssociationTicketDetails(string transIds, int? transSize, int? matchSize);

        AssociationOTGBViewModel GetAssociationByOTGB(long custID1, long custID2, AccountAssociationByOTGB fraudType);
    }
}