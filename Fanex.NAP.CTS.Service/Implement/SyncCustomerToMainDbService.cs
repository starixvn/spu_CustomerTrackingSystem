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
    using System.Collections.Generic;

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
    }
}
