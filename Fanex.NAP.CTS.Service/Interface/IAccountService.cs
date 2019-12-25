namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.Account;
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

        string GetShortestPath(int startCustId, int endCustId);

        IEnumerable<AssociatedDeviceByAccount> GetAssociatedDevicesByAccount(AssociatedDeviceByAccountFilter filter);

        List<SearchResultModel> SearchAccounts(string username);

        CustomerGeneralStatusModel GetCustomerGeneralStatus(int custId);

        CustomerInfoByCTSCustIdModel GetCustomerInfoByCTSCustId(int ctsCustId);

        CustomerAssociationByCTSCustIdModel GetCustomerTotalAssociationByCTSCustId(int ctsCustId);

        List<CustomerEvidenceByCTSCustIdModel> GetCustomerEvidenceByCTSCustId(int ctsCustId);

        List<CustomerStaticStatusModel> GetCustomerStaticStatus(int listId, bool status);

        CustomerProfileFraudInfoModel GetCustomerProfileFraudInfo(int custId);

        List<CustomerAssociationByAccountDetailModel> GetCustomerAssociationByAccountDetail(int ctsCustId, string statusIds, int associationType);

        CustomerLastTicketDateModel GetCustomerLastTicketDate(int custId);

        CustomerEsportAutoSuspendStatusModel GetCustomerEsportAutoSuspendedStatus(int custId);

        CustomerRobotUserStatusModel GetCustomerRobotUserStatus(int custId);
    }
}