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
        List<SearchResultModel> SearchAccounts(string username);

        List<CustomerGeneralStatusModel> GetCustomerGeneralStatus(string custIds);

        CustomerInfoByCTSCustIdModel GetCustomerInfoByCTSCustId(long ctsCustId);

        CustomerAssociationByCTSCustIdModel GetCustomerTotalAssociationByCTSCustId(long ctsCustId);

        List<CustomerEvidenceByCTSCustIdModel> GetCustomerEvidenceByCTSCustId(long ctsCustId);

        List<CustomerStaticStatusModel> GetCustomerStaticStatus(int listId, bool status);

        CustomerProfileFraudInfoModel GetCustomerProfileFraudInfo(long custId);

        List<TagModel> GetDirectAssociatedAccountTagAffected(string custIdsAssInfo);

        List<TagModel> GetDirectAssociatedAccountTagFlagged(string custIdsAssInfo);

        CustomerLastTicketDateModel GetCustomerLastTicketDate(long custId);

        CustomerEsportAutoSuspendStatusModel GetCustomerEsportAutoSuspendedStatus(long custId);

        CustomerRobotUserStatusModel GetCustomerRobotUserStatus(long custId);

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