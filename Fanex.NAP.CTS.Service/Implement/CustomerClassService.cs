using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.NAP.CTS.Core;
using Fanex.NAP.CTS.Core.BaseClasses;
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

        public DBResult InsertCustomerClass(string username, int custClass, bool isAffectToAssociation, int createdBy)
        {
            var criteria = new InsertSpecialCustomerClassCriteria()
            {
                Username = username,
                CustomerClass = custClass,
                CreatedBy = createdBy,
                IsAffectToAssociation = isAffectToAssociation
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                try
                {
                    db.ExecuteNonQuery(criteria);

                    return new DBResult
                    {
                        ErrorCode = 0,
                        ErrorMsg = string.Empty
                    };
                }
                catch (Exception ex)
                {
                    LogService.Error("CTS - Add Special Customer Class - exception: " + ex.Message, ex);

                    return new DBResult
                    {
                        ErrorCode = -1,
                        ErrorMsg = ex.Message
                    };
                }
            }            
        }
    }
}
