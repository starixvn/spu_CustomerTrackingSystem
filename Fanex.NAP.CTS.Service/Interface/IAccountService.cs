namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.Home;
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.ViewModel.Account;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.CTS.ViewModel.Account.GeneralInformation;
    using Fanex.NAP.CTS.ViewModel.Account.Search;
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

        CustomerInfoByCTSCustIdModel GetCustomerInfoByCTSCustId(int ctsCustId);

        CustomerAssociationByCTSCustIdModel GetCustomerTotalAssociationByCTSCustId(int ctsCustId);

        List<CustomerEvidenceByCTSCustIdModel> GetCustomerEvidenceByCTSCustId(int ctsCustId);

        List<CustomerStaticStatusModel> GetCustomerStaticStatus(int listId, bool status);

        CustomerProfileFraudInfoModel GetCustomerProfileFraudInfo(int custId);

        List<CustomerAssociationByAccountDetailModel> GetDirectCustomerAssociatedByAccount(string custIds);

        List<CustomerAssociationByAccountDetailModel> GetInDirectCustomerAssociatedByAccount(string custIds);

        CustomerLastTicketDateModel GetCustomerLastTicketDate(int custId);

        CustomerEsportAutoSuspendStatusModel GetCustomerEsportAutoSuspendedStatus(int custId);

        CustomerRobotUserStatusModel GetCustomerRobotUserStatus(int custId);

        CustomerFlagInfoModel GetCustomerFlagInfo(int ctsCustId, int subscriberId);

        List<GraphAssociationModel> GetCustomerAssociationByGraph(string apiURL, int ctsCustId);

        ShortestPathOfAccount GetShortestPathAssociationByGraph(string apiURL, int custIdFrom, int custIdTo);        

        List<GraphAssociationModel> FilterCustomerAssociationByStatus(string statusIds, List<GraphAssociationModel>  customerAssociation, List<CustomerGeneralStatusModel> statusByCustIds);

        List<GraphAssociationModel> FilterCustomerAssociationByAssociationType(int associationType, List<GraphAssociationModel> customerAssociation);

        IEnumerable<EvidenceModel> GetEvidenceList();

        IEnumerable<FlaggedEvidenceModel> GetFlaggedEvidences(int custId, string evidenceCodes);
    }
}