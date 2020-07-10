using Fanex.Data;
using Fanex.Logging;
using Fanex.NAP.CTS.Core;
using Fanex.NAP.CTS.Criteria.AssociatedAccount;
using Fanex.NAP.CTS.Criteria.AssociatedAccountMonitor;
using Fanex.NAP.CTS.Model.AssociatedAccountMonitor;
using Fanex.NAP.CTS.Service.Interface;
using Fanex.NAP.CTS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.CTS.Service.Implement
{
    public class AssociatedAccountService : IAssociatedAccountService
    {
        public IEnumerable<ValidateAssociatedAccountModel> ValidateAssociatedAccount(string toUserNameList, out bool isException)
        {
            isException = false;

            var criteria = new ValidateAssociatedAccountCriteria
            {
                UserNameList = toUserNameList
            };

            try
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    return db.Query<ValidateAssociatedAccountModel>(criteria).ToList();
                }
            }
            catch (Exception ex)
            {
                isException = true;
                LogService.Error("CTS - Validate manual associated account monitor - exception: " + ex.Message, ex);
                return GenerateAssociationErrorList(toUserNameList);
            }
        }

        private IEnumerable<ValidateAssociatedAccountModel> GenerateAssociationErrorList(string toUserNameList)
        {
            var errorList = new List<ValidateAssociatedAccountModel>();
            var userNameList = toUserNameList?.Split(',').AsEnumerable();

            if(userNameList != null)
            {
                errorList.AddRange(userNameList.Select(x => new ValidateAssociatedAccountModel
                {
                    UserName = x,
                    AccountStatus = 5
                }));
            }

            return errorList;
        }

        public DBResult InsertAssociatedAccountMultiple(int userID, string custIDs, string remark)
        {
            var criteria = new InsertAssociatedAccountCriteria
            {
                userID = userID,
                CTSCustIDs = custIDs,
                remark = remark
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
                    LogService.Error("CTS - Insert manual associated account - exception: " + ex.Message, ex);

                    return new DBResult
                    {
                        ErrorCode = -1,
                        ErrorMsg = ex.Message
                    };
                }
            }
        }

        public IEnumerable<AssociatedAccountMonitorModel> GetAssociatedAccountMonitor()
        {
            var criteria = new AssociatedAccountMonitorGetCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<AssociatedAccountMonitorModel>(criteria);
            }
        }

        public DBResult RemoveAssociatedAccount(long ctsCustId, int userID)
        {

            var criteria = new RemoveAssociatedAccountCriteria
            {
                CTSCustId = ctsCustId,
                UserID = userID
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);

                if (string.IsNullOrWhiteSpace(criteria.ErrorMessage))
                {
                    return new DBResult
                    {
                        ErrorCode = 0,
                        ErrorMsg = criteria.ErrorMessage
                    };
                }
            }

            return new DBResult
            {
                ErrorCode = -1,
                ErrorMsg = criteria.ErrorMessage
            };
        }

    }
}