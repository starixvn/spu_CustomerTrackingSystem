namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.ProblemAccountManagement;
    using Fanex.NAP.CTS.Criteria.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model.API.UpdateCategoryWhenPAUpdated;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountManagement;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            var result = Enumerable.Empty<ProblemAccountValidatedModel>();
            var criteria = new ValidateUserNamesInputCriteria
            {
                UserNames = model.UserNames,
                CategoryID = model.CategoryID,
                IsInsertingPA = model.PAAction == Model.ProblemAccountAction.Mark ? true : false
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var data = db.Query<ProblemAccountValidatedModel>(criteria);

                if (data != null && data.Any())
                {
                    result = data.Select(x => new ProblemAccountValidatedModel
                    {
                        UserName = x.UserName,
                        ActionID = x.ActionID,
                        Comment = model.Comment,
                        CustID = x.CustID,
                        ReasonID = x.ReasonID,
                        RoleID = x.RoleID,
                        Site = x.Site,                        
                        ErrorMsg = x.ErrorMsg
                    });
                }
            }

            return result;
        }

        public IEnumerable<GetCustEvidenceByReasonIdModel> GetCustEvidenceByReasonIds()
        {
            var criteria = new GetCustEvidenceByReasonIdCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetCustEvidenceByReasonIdModel>(criteria).ToList();
            }
        }

        private ProblemAccountAddCustomerModel GetProblemAccountAddCustomer(IEnumerable<ProblemAccountValidatedModel> models, int roleID)
        {
            var custs = models.Where(x => x.RoleID == roleID);
            var custFirst = custs.FirstOrDefault();

            if (custFirst != null)
            {
                return new ProblemAccountAddCustomerModel
                {
                    CustomerList = string.Join(",", custs.Select(x => x.UserName)),
                    CustIds = custs.Select(x => x.CustID),
                    RoleId = roleID,
                    ReasonId = custFirst.ReasonID,
                    PreventActionId = custFirst.ActionID,
                    Comment = custFirst.Comment
                };
            }

            return null;
        }
        
        public IEnumerable<ResultProblemAccountAddCustomerModel> MarkProblemAccountSiteCredit(IEnumerable<ProblemAccountValidatedModel> models, int userID)
        {
            var paResultList = new List<ResultProblemAccountAddCustomerModel>();

            if (models.Any())
            {
                var paCustList = GetProblemAccount(models);

                foreach (var cust in paCustList)
                {
                    if (cust == null)
                        continue;

                    var markResult = MarkProblemAccountSiteCredit(cust, userID);

                    if (!string.IsNullOrEmpty(markResult.ErrMsg))
                    {
                        paResultList.Add(markResult);

                        var jsonCustResult = JsonConvert.SerializeObject(cust);
                        var jsonMarkResult = JsonConvert.SerializeObject(markResult);

                        LogService.Error($"CTS - Mark Problem Account Site Credit failed Custs: { jsonCustResult } - ErrMsg: { jsonMarkResult }");
                    }
                }
            }

            return paResultList;
        }

        public IEnumerable<ResultProblemAccountAddCustomerModel> MarkProblemAccountSiteLicensee(IEnumerable<ProblemAccountValidatedModel> models, int userID)
        {
            var paResultList = new List<ResultProblemAccountAddCustomerModel>();

            if (models.Any())
            {
                var paCustList = GetProblemAccount(models);

                foreach (var cust in paCustList)
                {
                    if (cust == null)
                        continue;

                    var markResult = MarkProblemAccountSiteLicensee(cust, userID);

                    if (!string.IsNullOrEmpty(markResult.ErrMsg))
                    {
                        paResultList.Add(markResult);

                        var jsonCustResult = JsonConvert.SerializeObject(cust);
                        var jsonMarkResult = JsonConvert.SerializeObject(markResult);

                        LogService.Error($"CTS - Mark Problem Account Site Licensee failed Custs: { jsonCustResult } - ErrMsg: { jsonMarkResult }");
                    }
                }
            }

            return paResultList;
        }

        private ResultProblemAccountAddCustomerModel MarkProblemAccountSiteCredit(ProblemAccountAddCustomerModel model, int userID)
        {            
            var criteria = new ProblemAccountAddCustomerCriteria
            {
                CustomerList = model.CustomerList,
                RoleId = model.RoleId,
                ReasonId = model.ReasonId,
                PreventActionId = model.PreventActionId,
                Comment = model.Comment,
                UserID = userID,
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

                    LogService.Error($"CTS - Mark Problem Account Site Credit: { ex.Message } - data: { jsonModel }", ex);

                    return new ResultProblemAccountAddCustomerModel
                    {
                        ErrCode = -1,
                        ErrMsg = ex.Message
                    };
                }
            }
        }

        private ResultProblemAccountAddCustomerModel MarkProblemAccountSiteLicensee(ProblemAccountAddCustomerModel model, int UserID)
        {          
            var criteria = new ProblemAccountLicenseeAddCustomerCriteria
            {
                CustomerList = model.CustomerList,
                ReasonId = model.ReasonId,
                ActionId = model.PreventActionId,
                Comment = model.Comment,
                UserID = UserID,
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

                    LogService.Error($"CTS - Mark Problem Account Site Licensee: { ex.Message } - data: { jsonModel }", ex);

                    return new ResultProblemAccountAddCustomerModel
                    {
                        ErrCode = -1,
                        ErrMsg = ex.Message
                    };
                }
            }
        }      

        public IEnumerable<ResultProblemAccountAddCustomerModel> UpdateProblemAccountSiteCredit(IEnumerable<ProblemAccountValidatedModel> models, int userID)
        {
            var paResultList = new List<ResultProblemAccountAddCustomerModel>();

            if (models.Any())
            {
                var paCustList = GetProblemAccount(models);

                foreach (var cust in paCustList)
                {
                    if (cust == null)
                        continue;

                    var updateResult = UpdateProblemAccountSiteCredit(cust, userID);

                    if (!string.IsNullOrEmpty(updateResult.ErrMsg))
                    {
                        paResultList.Add(updateResult);

                        var jsonCustResult = JsonConvert.SerializeObject(cust);
                        var jsonMarkResult = JsonConvert.SerializeObject(updateResult);

                        LogService.Error($"CTS - Update Problem Account Site Credit failed Custs: { jsonCustResult } - ErrMsg: { jsonMarkResult }");
                    }
                }
            }

            return paResultList;
        }

        private ResultProblemAccountAddCustomerModel UpdateProblemAccountSiteCredit(ProblemAccountAddCustomerModel model, int userID)
        {
            var repository = new DynamicRepository();
            
            var criteria = new ProblemAccountUpdateCustomerCriteria
            {
                CustomerList = model.CustomerList,
                RoleId = model.RoleId,
                ReasonId = model.ReasonId,
                PreventActionId = model.PreventActionId,
                Comment = model.Comment,
                UserID = userID,
                IP = string.Empty
            };

            try
            {
                var resultUpdated = repository.Fetch<ResultProblemAccountUpdateModel>(criteria);

                UpdateCategories(resultUpdated, model.ReasonId, userID);

                return new ResultProblemAccountAddCustomerModel
                {
                    ErrCode = criteria.ErrCode,
                    ErrMsg = criteria.ErrMsg
                };
            }
            catch (Exception ex)
            {
                var jsonModel = JsonConvert.SerializeObject(model);

                LogService.Error($"CTS - Update Problem Account Site Credit: { ex.Message } - data: { jsonModel }", ex);

                return new ResultProblemAccountAddCustomerModel
                {
                    ErrCode = -1,
                    ErrMsg = ex.Message
                };
            }
        }

        public IEnumerable<ResultProblemAccountAddCustomerModel> UpdateProblemAccountSiteLicensee(IEnumerable<ProblemAccountValidatedModel> models, int userID)
        {
            var paResultList = new List<ResultProblemAccountAddCustomerModel>();

            if (models.Any())
            {
                var paCustList = GetProblemAccount(models);

                foreach (var cust in paCustList)
                {
                    if (cust == null)
                        continue;

                    var updateResult = UpdateProblemAccountSiteLicensee(cust, userID);

                    if (!string.IsNullOrEmpty(updateResult.ErrMsg))
                    {
                        paResultList.Add(updateResult);

                        var jsonCustResult = JsonConvert.SerializeObject(cust);
                        var jsonMarkResult = JsonConvert.SerializeObject(updateResult);

                        LogService.Error($"CTS - Update Problem Account Site Licensee failed Custs: { jsonCustResult } - ErrMsg: { jsonMarkResult }");
                    }
                }
            }

            return paResultList;
        }

        private ResultProblemAccountAddCustomerModel UpdateProblemAccountSiteLicensee(ProblemAccountAddCustomerModel model, int userID)
        {
            var repository = new DynamicRepository();

            var criteria = new ProblemAccountLicenseeUpdateCustomerCriteria
            {
                CustomerList = model.CustomerList,
                ReasonID = model.ReasonId,
                ActionId = model.PreventActionId,
                Comment = model.Comment,
                UserID = userID,
                IP = string.Empty
            };

            try
            {
                var resultUpdated = repository.Fetch<ResultProblemAccountUpdateModel>(criteria);

                UpdateCategories(resultUpdated, model.ReasonId, userID);

                return new ResultProblemAccountAddCustomerModel
                {
                    ErrCode = criteria.ErrCode,
                    ErrMsg = criteria.ErrMsg
                };
            }
            catch (Exception ex)
            {
                var jsonModel = JsonConvert.SerializeObject(model);

                LogService.Error($"CTS - Update Problem Account Site Licensee: { ex.Message } - data: { jsonModel }", ex);

                return new ResultProblemAccountAddCustomerModel
                {
                    ErrCode = -1,
                    ErrMsg = ex.Message
                };
            }
        }
        
        public IEnumerable<ResultProblemAccountAddCustomerModel> UnmarkProblemAccountSiteCredit(IEnumerable<ProblemAccountValidatedModel> models, bool isExceptDownlinesByTVS, bool isExceptDownlinesByOPS, int userID)
        {
            var paResultList = new List<ResultProblemAccountAddCustomerModel>();

            if (models.Any())
            {
                var paCustList = GetProblemAccount(models);

                foreach (var cust in paCustList)
                {
                    if (cust == null)
                        continue;

                    var unmarkResult = UnmarkProblemAccountSiteCredit(cust, isExceptDownlinesByTVS, isExceptDownlinesByOPS, userID);

                    if (!string.IsNullOrEmpty(unmarkResult.ErrMsg))
                    {
                        paResultList.Add(unmarkResult);

                        var jsonCustResult = JsonConvert.SerializeObject(cust);
                        var jsonMarkResult = JsonConvert.SerializeObject(unmarkResult);

                        LogService.Error($"CTS - Unmark Problem Account Site Credit failed Custs: { jsonCustResult } - ErrMsg: { jsonMarkResult }");
                    }
                }
            }

            return paResultList;
        }

        private ResultProblemAccountAddCustomerModel UnmarkProblemAccountSiteCredit(ProblemAccountAddCustomerModel model, bool isExceptDownlinesByTVS, bool isExceptDownlinesByOPS, int userID)
        {
            var repository = new DynamicRepository();

            var criteria = new ProblemAccountRemoveCustomerCriteria
            {
                CustomerList = model.CustomerList,
                RoleID = model.RoleId,
                ExcludeAbnormalMember = isExceptDownlinesByTVS,
                ExcludeDirectMark = isExceptDownlinesByOPS,
                UserID = userID,
                IP = string.Empty
            };

            try
            {
                var resultUpdated = repository.Execute(criteria);

                if (string.IsNullOrEmpty(criteria.ErrMsg))
                {
                    if (!string.IsNullOrEmpty(criteria.SuccessAllDownlineList))
                    {
                        var custs = criteria.SuccessAllDownlineList.Split(',').AsEnumerable();
                        RemoveProblemAccounts(custs, userID);
                    }
                    
                }                    
                
                return new ResultProblemAccountAddCustomerModel
                {
                    ErrCode = criteria.ErrCode,
                    ErrMsg = criteria.ErrMsg
                };
            }
            catch (Exception ex)
            {
                var jsonModel = JsonConvert.SerializeObject(model);

                LogService.Error($"CTS - Unmark Problem Account Site Credit: { ex.Message } - data: { jsonModel }", ex);

                return new ResultProblemAccountAddCustomerModel
                {
                    ErrCode = -1,
                    ErrMsg = ex.Message
                };
            }
        }

        public IEnumerable<ResultProblemAccountAddCustomerModel> UnmarkProblemAccountSiteLicensee(IEnumerable<ProblemAccountValidatedModel> models, int userID)
        {
            var paResultList = new List<ResultProblemAccountAddCustomerModel>();

            if (models.Any())
            {
                var paCustList = GetProblemAccount(models);

                foreach (var cust in paCustList)
                {
                    if (cust == null)
                        continue;

                    var unmarkResult = UnmarkProblemAccountSiteLicensee(cust, userID);

                    if (!string.IsNullOrEmpty(unmarkResult.ErrMsg))
                    {
                        paResultList.Add(unmarkResult);

                        var jsonCustResult = JsonConvert.SerializeObject(cust);
                        var jsonMarkResult = JsonConvert.SerializeObject(unmarkResult);

                        LogService.Error($"CTS - Unmark Problem Account Site Licensee failed Custs: { jsonCustResult } - ErrMsg: { jsonMarkResult }");
                    }
                }
            }

            return paResultList;
        }

        private ResultProblemAccountAddCustomerModel UnmarkProblemAccountSiteLicensee(ProblemAccountAddCustomerModel model, int userID)
        {
            var repository = new DynamicRepository();

            var criteria = new ProblemAccountLicenseeRemoveCustomerCriteria
            {
                CustomerList = model.CustomerList,
                UserID = userID,
                IP = string.Empty
            };

            try
            {
                var resultUpdated = repository.Execute(criteria);

                if (string.IsNullOrEmpty(criteria.ErrMsg))
                {
                    if (model.CustIds != null && model.CustIds.Any())
                    {
                        var custs = model.CustIds.Select(x => x.ToString());

                        RemoveProblemAccounts(custs, userID);
                    }
                }

                return new ResultProblemAccountAddCustomerModel
                {
                    ErrCode = criteria.ErrCode,
                    ErrMsg = criteria.ErrMsg
                };
            }
            catch (Exception ex)
            {
                var jsonModel = JsonConvert.SerializeObject(model);

                LogService.Error($"CTS - Unmark Problem Account Site Licensee: { ex.Message } - data: { jsonModel }", ex);

                return new ResultProblemAccountAddCustomerModel
                {
                    ErrCode = -1,
                    ErrMsg = ex.Message
                };
            }
        }

        private List<ProblemAccountAddCustomerModel> GetProblemAccount(IEnumerable<ProblemAccountValidatedModel> models)
        {
            var paCustList = new List<ProblemAccountAddCustomerModel>();
            paCustList.Add(GetProblemAccountAddCustomer(models, 4)); // Supper
            paCustList.Add(GetProblemAccountAddCustomer(models, 3)); // Master
            paCustList.Add(GetProblemAccountAddCustomer(models, 2)); // Agent
            paCustList.Add(GetProblemAccountAddCustomer(models, 1)); // Member

            return paCustList;
        }

        private void UpdateCategories(IEnumerable<ResultProblemAccountUpdateModel> paUpdateds, int toReasonID, int userID)
        {
            var apiEndPoint = ConfigSettings.UpdateCategories;

            var custInfos = paUpdateds.Select(x => new CustInfoModel
            {
                CustID = x.custId,
                FromReasonID = x.OldReasonId,
                ToReasonID = toReasonID
            });

            try
            {               
                var paras = new UpdateCategoryParamsModel
                {
                    CustInfos = custInfos,
                    UserId = userID
                };

                var data = ApiHelper.Post<ApiResultModel>(apiEndPoint, paras);

                Task.WhenAll(data);
            }
            catch (Exception ex)
            {
                var jsonModel = JsonConvert.SerializeObject(custInfos);

                LogService.Error($"CTS - Update Categories failed: { ex.Message } - data: {jsonModel}" + ex.Message, ex);
            }
        }

        private void RemoveProblemAccounts(IEnumerable<string> custIdList, int userID)
        {
            try
            {
                var apiEndPoint = ConfigSettings.RemoveProblemAccounts;
                var custIds = string.Join(",", custIdList);

                var data = ApiHelper.Post<ApiResultModel>(apiEndPoint, custIds);

                Task.WhenAll(data);
            }
            catch (Exception ex)
            {                
                LogService.Error($"CTS - Remove Problem Account failed: { ex.Message } - custIds: {custIdList}" + ex.Message, ex);
            }
        }

        public IEnumerable<long> UpdateProblemAccountEvidence(UpdateCategoryParamsModel paras)
        {
            var custIdsJson = JsonConvert.SerializeObject(paras.CustInfos);
            var repository = new DynamicRepository();

            var criteria = new UpdateProblemAccountEvidenceCriteria
            {
                CustInfo = custIdsJson,
                UserID = paras.UserId,
                SourceType = 1
            };

            LogService.Info($"CTS Portal API - UpdateProblemAccountEvidence custIdsJson:  {custIdsJson}");

            return repository.Fetch<long>(criteria);
        }

        public IEnumerable<long> RemoveProblemAccountEvidence(string custIds)
        {
            var repository = new DynamicRepository();
            var criteria = new RemoveProblemAccountEvidenceCriteria
            {
                CustIDs = custIds,
                SourceType = 1
            };

            return repository.Fetch<long>(criteria);
        }
    }
}
