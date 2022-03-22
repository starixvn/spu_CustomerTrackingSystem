namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Criteria.ProblemAccountManagement;
    using Fanex.NAP.CTS.Criteria.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountManagement;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
       
        public IEnumerable<ProblemAccountCommentSelModel> GetProblemAccountCommentSel(string custIdList)
        {
            var criteria = new ProblemAccountCustomerCommentSelCriteria
            {
                CustIdList = custIdList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ProblemAccountCommentSelModel>(criteria).ToList();
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

        public IEnumerable<ProblemAccountValidatedModel> ValidateProblemAccounts(ValidateProblemAccountInputViewModel model)
        {
            var criteria = new ValidateUserNamesInputCriteria
            {
                UserNames = model.UserNames,
                CategoryID = model.CategoryID,
                IsInsertingPA = model.PAAction == Model.ProblemAccountAction.Mark ? true : false
            };

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

        public IEnumerable<ResultProblemAccountAddCustomerModel> MarkNewProblemAccount(IEnumerable<ProblemAccountValidatedModel> validatedModels)
        {
            var paAddResultList = new List<ResultProblemAccountAddCustomerModel>();
            var paAddCustList = new List<ProblemAccountAddCustomerModel>();
            var paForSiteCredit = validatedModels.Where(x => x.Site == Consts.ProblemAccountManagement_SiteCredit);

            if (paForSiteCredit.Any())
            {                
                paAddCustList.Add(GetProblemAccountAddCustomer(paForSiteCredit, 4)); // Supper
                paAddCustList.Add(GetProblemAccountAddCustomer(paForSiteCredit, 3)); // Master
                paAddCustList.Add(GetProblemAccountAddCustomer(paForSiteCredit, 2)); // Agent
                paAddCustList.Add(GetProblemAccountAddCustomer(paForSiteCredit, 1)); // Member

                foreach (var cust in paAddCustList)
                {
                    var markResult = MarkProblemAccountSiteCredit(cust);

                    if (!string.IsNullOrEmpty(markResult.ErrMsg))
                    {
                        paAddResultList.Add(markResult);

                        var jsonCustResult = JsonConvert.SerializeObject(cust);
                        var jsonMarkResult = JsonConvert.SerializeObject(markResult);

                        LogService.Error($"CTS - Mark Problem Account Site Credit failed Custs: { jsonCustResult } - ErrMsg: { jsonMarkResult }");
                    }
                }
            }

            paAddCustList.Clear();

            var paForSiteLicensee = validatedModels.Where(x => x.Site == Consts.ProblemAccountManagement_SiteLicensee);

            if (paForSiteLicensee.Any())
            {
                paAddCustList.Add(GetProblemAccountAddCustomer(paForSiteLicensee, 4)); // Supper
                paAddCustList.Add(GetProblemAccountAddCustomer(paForSiteLicensee, 3)); // Master
                paAddCustList.Add(GetProblemAccountAddCustomer(paForSiteLicensee, 2)); // Agent
                paAddCustList.Add(GetProblemAccountAddCustomer(paForSiteLicensee, 1)); // Member

                foreach (var cust in paAddCustList)
                {
                    var markResult = MarkProblemAccountSiteLicensee(cust);

                    if (!string.IsNullOrEmpty(markResult.ErrMsg))
                    {
                        paAddResultList.Add(markResult);

                        var jsonCustResult = JsonConvert.SerializeObject(cust);
                        var jsonMarkResult = JsonConvert.SerializeObject(markResult);

                        LogService.Error($"CTS - Mark Problem Account Site Licensee failed Custs: { jsonCustResult } - ErrMsg: { jsonMarkResult }");
                    }
                }
            }

            return paAddResultList;
        }

        private ProblemAccountAddCustomerModel GetProblemAccountAddCustomer(IEnumerable<ProblemAccountValidatedModel> validatedModels, int roleID)
        {            
            var custs = validatedModels.Where(x => x.RoleID == roleID);
            var custFirst = custs.FirstOrDefault();

            if (custFirst != null)
            {
                return new ProblemAccountAddCustomerModel
                {
                    CustomerList = string.Join(",", custs.Select(x => x.UserName)),
                    RoleId = roleID,
                    ReasonId = custFirst.ReasonID,
                    PreventActionId = custFirst.ActionID,
                    Comment = custFirst.Comment,
                    UserID = custFirst.UserID
                };
            }

            return null;
        }

        private ResultProblemAccountAddCustomerModel MarkProblemAccountSiteCredit(ProblemAccountAddCustomerModel model)
        {
            if (model == null)
            {
                return new ResultProblemAccountAddCustomerModel { ErrCode = -1, ErrMsg = string.Empty };
            }

            var criteria = new ProblemAccountAddCustomerCriteria
            {
                CustomerList = model.CustomerList,
                RoleId = model.RoleId,
                ReasonId = model.ReasonId,
                PreventActionId = model.PreventActionId,
                Comment = model.Comment,
                UserID= model.UserID,
                IP = string.Empty
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                try
                {
                    db.ExecuteNonQuery(criteria);

                    return new ResultProblemAccountAddCustomerModel
                    {
                        ErrCode = criteria.ErrCode,
                        ErrMsg = criteria.ErrMsg,
                        SuccessList = criteria.SuccessList,
                        FailList = criteria.FailList
                    };
                }
                catch (Exception ex)
                {
                    var jsonModel = JsonConvert.SerializeObject(model);

                    LogService.Error($"CTS - Mark Problem Account Site Credit Credit: { ex.Message } - data: { jsonModel }", ex);

                    return new ResultProblemAccountAddCustomerModel
                    {
                        ErrCode = -1,
                        ErrMsg = ex.Message                        
                    };
                }               
            }
        }

        private ResultProblemAccountAddCustomerModel MarkProblemAccountSiteLicensee(ProblemAccountAddCustomerModel model)
        {
            if (model == null)
            {
                return new ResultProblemAccountAddCustomerModel { ErrCode = -1, ErrMsg = string.Empty };
            }

            var criteria = new ProblemAccountLicenseeAddCustomerCriteria
            {
                CustomerList = model.CustomerList,
                ReasonId = model.ReasonId,
                ActionId = model.PreventActionId,
                Comment = model.Comment,
                UserID = model.UserID,
                IP = string.Empty
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                try
                {
                    db.ExecuteNonQuery(criteria);

                    return new ResultProblemAccountAddCustomerModel
                    {
                        ErrCode = criteria.ErrCode,
                        ErrMsg = criteria.ErrMsg
                    };
                }
                catch (Exception ex)
                {
                    var jsonModel = JsonConvert.SerializeObject(model);

                    LogService.Error($"CTS - Mark Problem Account Site Credit Licensee: { ex.Message } - data: { jsonModel }", ex);

                    return new ResultProblemAccountAddCustomerModel
                    {
                        ErrCode = -1,
                        ErrMsg = ex.Message
                    };
                }
            }
        }
    }
}
