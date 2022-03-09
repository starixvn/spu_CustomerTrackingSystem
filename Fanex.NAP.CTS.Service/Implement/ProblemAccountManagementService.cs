namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Criteria.ProblemAccountManagement;
    using Fanex.NAP.CTS.Criteria.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ProblemAccountManagementService : IProblemAccountManagementService
    {
        public IEnumerable<SubscriberGroupModel> GetAllSites()
        {
            var criteria = new GetAllSubscriberGroupsCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<SubscriberGroupModel>(criteria);
            }
        }

        public IEnumerable<ProblemAccountModel> GetProblemAccounts(ProblemAccountFilterViewModel filter)
        {
            var criteria = new GetProblemAccountCriteria
            {
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                Sites = filter.Sites,
                UserName = filter.UserName,
                HasDownline = filter.HasDownline
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ProblemAccountModel>(criteria).ToList();
            }
        }
       
        public IEnumerable<ProblemAccountCategoryModel> GetProblemAccountCategories()
        {
            var criteria = new GetProblemAccountCategoryCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ProblemAccountCategoryModel>(criteria).ToList();
            }
        }

        public IEnumerable<ProblemAccountValidatedModel> ValidateProblemAccounts()
        {
            var criteria = new ValidateUserNamesInputCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ProblemAccountValidatedModel>(criteria).ToList();
            }        
        }

        public IEnumerable<GetCustEvidenceByReasonIdModel> GetCustEvidenceByReasonIds()
        {
            var criteria = new GetCustEvidenceByReasonIdCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetCustEvidenceByReasonIdModel>(criteria).ToList();
            }
        }

    }
}
