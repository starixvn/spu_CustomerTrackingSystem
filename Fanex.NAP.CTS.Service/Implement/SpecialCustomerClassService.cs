namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.Logging;    
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Criteria.CustomerClass;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using System;
    using System.Collections.Generic;

    public class SpecialCustomerClassService : ISpecialCustomerClassService
    {
        public IEnumerable<SpecialCustomerClassModel> GetAllCustomerClass()
        {
            var criteria = new GetAllSpecialCustomerClassCriteria()
            {
                IsGetAllInfo = true
            };

            return DataService.GetData<SpecialCustomerClassModel>(criteria);
        }

        public InsertSpecialCustomerClassResult InsertCustomerClass(string username, int custClass, bool isAffectToAssociation, int createdBy)
        {
            var insertCTSResult = new InsertSpecialCustomerClassResult();
            var criteria = new InsertSpecialCustomerClassCriteria()
            {
                Username = username,
                CustomerClass = custClass,
                CreatedBy = createdBy,
                IsAffectToAssociation = isAffectToAssociation
            };

            var repository = new DynamicRepository();
            insertCTSResult.CustomerClassList = repository.Fetch<SpecialCustomerClassForMainDb>(criteria);
            insertCTSResult.OpErrorMessage = criteria.ErrorMessage;

            return insertCTSResult;
        }

        public int SyncCustomerClassToMainDb(IEnumerable<SpecialCustomerClassForMainDb> customerClassList)
        {
            var insertCode = 0;

            var criteria = new SyncCustomerClassToMainDbCriteria()
            {
                custList = customerClassList.ToXmlString()
            };

            try
            {
                var repository = new DynamicRepository();
                insertCode = repository.Execute(criteria);
            }
            catch (Exception ex)
            {
                var errorMessage = "CTS - SyncCustomerClassToMainDb exception ";
                LogService.Error(errorMessage, ex);
            }

            return insertCode;
        }

        public int RemoveCustomerClass(long ctsCustId)
        {
            return 0;
        }
    }
}
