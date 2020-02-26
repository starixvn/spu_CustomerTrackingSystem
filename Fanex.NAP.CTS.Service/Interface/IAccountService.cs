namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.Home;
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.ViewModel.Account;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.CTS.ViewModel.Account.GeneralInformation;
    using Fanex.NAP.CTS.ViewModel.Account.Search;
    using System;
    using System.Collections.Generic;

    public interface IAccountService
    {
        IEnumerable<SubscriberAssignment> GetSubscribers(long userId);

        bool CheckIsMasterSubscriber(IEnumerable<SubscriberAssignment> subscribers);

        bool CheckIsPermissionCN88(IEnumerable<SubscriberAssignment> subscribers);

        IEnumerable<SubscriberAccount> SearchAccounts(SearchAccountFilter filter);

        AccountDetail GetAccountDetailGeneralInformation(GeneralInformationFilter filter);

        IEnumerable<AssociatedAccountByAccount> GetAssociatedAccountByAccounts(AssociatedAccountByAccountFilter filter, out string msgError);        

        IEnumerable<AssociatedDeviceByAccount> GetAssociatedDevicesByAccount(AssociatedDeviceByAccountFilter filter);


        // New Implement


        List<SearchResultModel> SearchAccounts(string username);

        List<CustomerGeneralStatusModel> GetCustomerGeneralStatus(string custIds);

        CustomerInfoByCTSCustIdModel GetCustomerInfoByCTSCustId(Int64 ctsCustId);

        CustomerAssociationByCTSCustIdModel GetCustomerTotalAssociationByCTSCustId(Int64 ctsCustId);

        List<CustomerEvidenceByCTSCustIdModel> GetCustomerEvidenceByCTSCustId(Int64 ctsCustId);

        List<CustomerStaticStatusModel> GetCustomerStaticStatus(int listId, bool status);

        CustomerProfileFraudInfoModel GetCustomerProfileFraudInfo(int custId);

        List<TagModel> GetDirectAssociatedAccountTagAffected(string custIdsAssInfo);

        List<TagModel> GetDirectAssociatedAccountTagFlagged(string custIdsAssInfo);

        CustomerLastTicketDateModel GetCustomerLastTicketDate(int custId);

        CustomerEsportAutoSuspendStatusModel GetCustomerEsportAutoSuspendedStatus(int custId);

        CustomerRobotUserStatusModel GetCustomerRobotUserStatus(int custId);

        CustomerFlagInfoModel GetCustomerFlagInfo(long ctsCustId, int subscriberId);

        List<GraphAssociationModel> GetCustomerAssociationByGraph(string apiURL, long ctsCustId);

        List<GraphDirectAssociationModel> GetDirectAssocationByGraph(string apiURL, List<long> ctsCustIds);

        ShortestPathOfAccount GetShortestPathAssociationByGraph(string apiURL, int custIdFrom, int custIdTo);        

        List<GraphAssociationModel> FilterCustomerAssociationByStatus(string statusIds, List<GraphAssociationModel>  customerAssociation, List<CustomerGeneralStatusModel> statusByCustIds);

        List<GraphAssociationModel> FilterCustomerAssociationByAssociationType(int associationType, List<GraphAssociationModel> customerAssociation);

        IEnumerable<EvidenceModel> GetEvidenceList();

        IEnumerable<FlaggedEvidenceModel> GetFlaggedEvidences(long ctsCustId, string evidenceCodes);

        AffectedRetractEvidenceModel GetAffectedRetractEvidences(long ctsCustId, string evidenceCodes);

        DBResult PlaceEvidence(UpdatePlaceEvidenceModel model);

        DBResult RemoveFlaggedEvidence(RemoveEvidenceModel model);

        List<AccountExceptionsModel> GetAccountExceptions(long ctsCustId);

        int AddAccountException(UpdateAccountExceptionModel model);

        int RemoveAccountException(UpdateAccountExceptionModel model);

        int RetractAffectedEvidence(RemoveEvidenceModel model);

        int RestoreAffectedEvidence(RemoveEvidenceModel model);

        IEnumerable<AffectedEvidenceListBetweenCustsModel> GetAffectedEvidenceListBetweenCusts(Int64 fromCTSCustID, Int64 toCTSCustID);        
    }
}