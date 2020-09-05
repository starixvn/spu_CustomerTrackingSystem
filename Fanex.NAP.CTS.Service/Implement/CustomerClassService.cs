using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.NAP.CTS.Core;
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

        public InsertCustomerClassResult InsertCustomerClass(string username, int custClass, bool isAffectToAssociation, int createdBy)
        {
            var insertCTSResult = new InsertCustomerClassResult();
            var criteria = new InsertSpecialCustomerClassCriteria()
            {
                Username = username,
                CustomerClass = custClass,
                CreatedBy = createdBy,
                IsAffectToAssociation = isAffectToAssociation
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var query = db.QueryMultiple(criteria))
                {
                    insertCTSResult.CustomerClassList = query.Read<CustomerClassForMainDb>();
                    var opErrorMessage = query.Read<string>();

                    if (opErrorMessage != null && opErrorMessage.Any())
                    {
                        insertCTSResult.OpErrorMessage = opErrorMessage.ToList()[0];
                    }                    

                    return insertCTSResult;
                }
            }

            //var repository = new DynamicRepository();
            //var customerClassList = repository.Fetch<CustomerClassForMainDb>(criteria);

            //return customerClassList;
        }

        public int SyncCustomerClassToMainDb(IEnumerable<CustomerClassForMainDb> customerClassList)
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
    }
}
