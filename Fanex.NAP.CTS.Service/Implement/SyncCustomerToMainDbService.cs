namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Criteria.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Service.Interface;
    using System;
    using Fanex.Data;
    using System.Collections.Generic;
    using System.Linq;

    public class SyncCustomerToMainDbService : ISyncCustomerToMainDbService
    {
        public DBResult SyncCustomerClassToMainDb(IEnumerable<CustomerClassForMainDb> customerClassList, string syncAction)
        {
            var criteria = new SyncCustomerClassToMainDbCriteria
            {
                custList = customerClassList.ToXmlString()               
            };

            try
            {
                var repository = new DynamicRepository();

                repository.Execute(criteria);

                if (criteria.ErrorCode != 0)
                {
                    var errorMessage = "CTS - " + syncAction + " - SyncCustomerClassToMainDb fail:";
                    errorMessage += " errorCode = " + criteria.ErrorCode;
                    errorMessage += "; customerClassList = " + criteria.custList;

                    LogService.Error(errorMessage);

                    return new DBResult { ErrorCode = criteria.ErrorCode, ErrorMsg = criteria.ErrorMessage };
                }

                var feedDataToSoon88Service = new FeedDataToSoon88Service();
                feedDataToSoon88Service.FeedDataToSoon88(customerClassList);
            }
            catch (Exception ex)
            {
                var errorMessage = "CTS - " + syncAction + " - SyncCustomerClassToMainDb exception:";
                errorMessage += " customerClassList = " + criteria.custList;

                LogService.Error(errorMessage, ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        public DBResult SyncCustomerClassToMainDb(string ctsCustID, bool isCustId, bool isExcludedDetailedCategory)
        {
            var result = new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };

            if (string.IsNullOrEmpty(ctsCustID))
            {
                return result;
            }

            var custEvidenceCategory = GetLatestEvidenceCategory(ctsCustID, isCustId, isExcludedDetailedCategory).Where(c => c.CategoryIdMapping != -99);

            if (custEvidenceCategory == null || !custEvidenceCategory.Any())
            {
                return result;
            }

            var customerClassList = custEvidenceCategory.Select(c => new CustomerClassForMainDb
            {
                CustID = c.CustID,
                CustomerClass = c.CategoryIdMapping
            }).Distinct();

            return SyncCustomerClassToMainDb(customerClassList, "Update Evidence Category");
        }

        public IEnumerable<EvidenceCategoryToCustomerClassForMainDb> GetLatestEvidenceCategory(string ctsCustIDs, bool isCustId, bool isExcludedDetailedCategory)
        {
            var criteria = new GetLastestEvidenceCategoryCriteria()
            {
                CustIDList = ctsCustIDs,
                IsUsingCustID = isCustId,
                IsExcludedDetailedCategory = isExcludedDetailedCategory

            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<EvidenceCategoryToCustomerClassForMainDb>(criteria).ToList();
            }
        }
    }
}
