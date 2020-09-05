using Fanex.Data.Repository;
using Fanex.NAP.CTS.Core.BaseClasses;
using Fanex.NAP.CTS.Core.Helpers.Xml;
using Fanex.NAP.CTS.Criteria.CustomerClass;
using Fanex.NAP.CTS.Model.CustomerClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Service.Implement
{
    public class CustomerClassService
    {
        public IEnumerable<CustomerClassModel> GetAllCustomerClass()
        {
            var criteria = new GetAllSpecialCustomerClassCriteria()
            {
                IsGetAllInfo = true
            };

            return DataService.GetData<CustomerClassModel>(criteria);
        }

        public IEnumerable<CustomerClassForMainDb> InsertCustomerClass(string username, int custClass, bool isAffectToAssociation, int createdBy)
        {
            var criteria = new InsertSpecialCustomerClassCriteria()
            {
                Username = username,
                CustomerClass = custClass,
                CreatedBy = createdBy,
                IsAffectToAssociation = isAffectToAssociation
            };

            var repository = new DynamicRepository();
            var customerClassList = repository.Fetch<CustomerClassForMainDb>(criteria);
            return customerClassList;
        }

        public void SyncCustomerClassToMainDb(IEnumerable<CustomerClassForMainDb> customerClassList)
        {
            var criteria = new SyncCustomerClassToMainDbCriteria()
            {
                custList = customerClassList.ToXmlString()
            };

            var repository = new DynamicRepository();
            repository.Execute(criteria);
        }
    }
}
