namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Criteria.Account.Associated;
    using Fanex.NAP.CTS.Criteria.AssociationReport;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.AssociationReport;
    using Fanex.NAP.CTS.ViewModel.AssociationReport;
    using System.Collections.Generic;
    using System.Linq;

    public class AssociationReportService : IAssociationReportService
    {
        private readonly ICriteriaService _repository;

        public AssociationReportService()
        {
            _repository = new CriteriaService();
        }

        #region Query From MySQL

        public List<CustomerStaticStatusModel> GetCustomerStaticStatus(int listId, bool status)
        {
            var criteria = new CustomerStaticAccountStatusCriteria
            {
                ListId = listId,
                Status = status
            };

            var result = _repository.Fetch<CustomerStaticStatusModel>(criteria);
            return result.ToList();
        }

        public IEnumerable<AccountStaticStatusModel> GetAccountStaticStatuses()
        {
            var statusList = new List<AccountStaticStatusModel>();

            statusList.Add(new AccountStaticStatusModel
            {
                Ids = "1,11", // 1:Open, 11:Active
                Name = "Open"
            });

            statusList.Add(new AccountStaticStatusModel
            {
                Ids = "2,3,4,12,13,14", // 2:Disabled, 3:Closed, 4:Suspended, 12:Inactive, 13:View Only, 14:Suspended
                Name = "Closed"
            });

            return statusList;
        }

        public IEnumerable<EvidenceModel> GetEvidenceList()
        {
            var criteria = new GetAllEvidenceCriteria();
            return _repository.Fetch<EvidenceModel>(criteria);
        }

        public DBResult ActiveRemoveAssociation(int fromSubscriberId, long fromCTSCustId, int toSubscriberId, long toCTSCustId, int createdBy, bool isRemoveAction)
        {
            var criteria = new ActiveRemoveAssociationCriteria
            {
                FromSubscriberId = fromSubscriberId,
                FromCTSCustId = fromCTSCustId,
                ToSubscriberId = toSubscriberId,
                ToCTSCustId = toCTSCustId,
                CreatedBy = createdBy,
                IsActionRemove = isRemoveAction
            };

            _repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrWhiteSpace(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage
            };
        }

        public IEnumerable<AssociationFromMySqlModel> GetAssociationReportFromMySql(AssociationFilterViewModel filter, out int totalItems)
        {
            totalItems = 0;

            var criteria = new AssociationReportGetCriteria
            {
                FromAssDate = filter.FromDate,
                ToAssDate = filter.ToDate,
                AccountStatusIDs = filter.AccountStatusIds,
                SubscriberIDs = filter.SubscriberIDs,
                AssociationStatus = filter.AssociationStatus.ToString(),
                EvidenceIDs = filter.EvidenceIDs,
                AssociationType = filter.AssociationType,
                TakeRow = filter.length,
                SkipRow = filter.start
            };

            var data = _repository.Fetch<AssociationFromMySqlModel>(criteria);
            totalItems = criteria.TotalItems;

            return data;
        }

        public IEnumerable<AssociationDetailFromMySqlModel> GetAssociationDetailFromMySql(AssociationDetailFilterViewModel filter, out int totalItems)
        {
            totalItems = 0;

            var criteria = new AssociationDetailReportGetCriteria
            {
                CTSCustID = filter.CTSCustId,
                FromAssDate = filter.FromDate,
                ToAssDate = filter.ToDate,
                AssociationType = filter.AssociationType,
                TakeRow = filter.length,
                SkipRow = filter.start
            };

            var data = _repository.Fetch<AssociationDetailFromMySqlModel>(criteria);
            totalItems = criteria.TotalItems;

            return data;
        }

        #endregion
    }
}
