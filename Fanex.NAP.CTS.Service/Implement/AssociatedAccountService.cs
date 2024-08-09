using Fanex.Data;
using Fanex.NAP.CTS.Model.DTO;
using Fanex.NAP.CTS.Service.Interface;
using System.Collections.Generic;
using Fanex.NAP.CTS.Criteria.Api;
using Fanex.NAP.CTS.Model.Account;

namespace Fanex.NAP.CTS.Service.Implement
{
    public class AssociatedAccountService : IAssociatedAccountService
    {        
        public IEnumerable<CustomerEvidenceForTW> GetFlaggedEvidenceByDirectAssForAPI(string custIdList)
        {
            var criteria = new GetFlaggedEvidenceByDirectAssociationCriteria()
            {
                CustIds = custIdList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerEvidenceForTW>(criteria);
            }
        }

        public IEnumerable<CustomerEvidenceForTW> GetFlaggedEvidenceByLevel2AssForAPI(string custIdList)
        {
            var criteria = new GetFlaggedEvidenceByLevel2AssociationCriteria()
            {
                CustIds = custIdList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerEvidenceForTW>(criteria);
            }
        }

        public IEnumerable<AssociatedAccountModel> GetAssociationForExtAcc(int custId)
        {
            var criteria = new GetAssociationForExtAccCriteria()
            {
                CustId = custId,
                AssStatus = 1,
                AssType = 1
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<AssociatedAccountModel>(criteria);
            }
        }
    }
}