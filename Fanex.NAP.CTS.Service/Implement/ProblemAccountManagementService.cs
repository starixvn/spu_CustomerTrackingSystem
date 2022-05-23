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
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using Fanex.NAP.TGRS.Core.ExcelUtility.RichTextExtensions;
    using Newtonsoft.Json;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
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

        //public IEnumerable<ProblemAccountCommentSelModel> GetProblemAccountCommentSel(string custIdList)
        //{
        //    var criteria = new ProblemAccountCustomerCommentSelCriteria
        //    {
        //        CustIdList = custIdList
        //    };

        //    using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
        //    {
        //        return db.Query<ProblemAccountCommentSelModel>(criteria).ToList();
        //    }
        //}

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
                UserNames = model.UserNames             
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var data = db.Query<ProblemAccountValidatedModel>(criteria);

                if (data != null && data.Any())
                {
                    result = data.Select(x => new ProblemAccountValidatedModel
                    {
                        CTSCustID = x.CTSCustID,
                        SubscriberID = x.SubscriberID,
                        RoleID = x.RoleID,
                        CustID = x.CustID,                        
                        UserName = x.UserName,
                        IsLicensee = x.IsLicensee,
                        ActionID = x.ActionID,
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
                    //ReasonId = custFirst.ReasonID,
                    PreventActionId = custFirst.ActionID,
                    //Comment = custFirst.Comment
                };
            }

            return null;
        }

        //public IEnumerable<ResultProblemAccountAddCustomerModel> MarkProblemAccountSiteCredit(IEnumerable<ProblemAccountValidatedModel> models, int userID)
        //{
        //    var paResultList = new List<ResultProblemAccountAddCustomerModel>();

        //    if (models.Any())
        //    {
        //        var paCustList = GetProblemAccount(models);

        //        foreach (var cust in paCustList)
        //        {
        //            if (cust == null)
        //                continue;

        //            var markResult = MarkProblemAccountSiteCredit(cust, userID);

        //            if (!string.IsNullOrEmpty(markResult.ErrMsg))
        //            {
        //                paResultList.Add(markResult);

        //                var jsonCustResult = JsonConvert.SerializeObject(cust);
        //                var jsonMarkResult = JsonConvert.SerializeObject(markResult);

        //                LogService.Error($"CTS - Mark Problem Account Site Credit failed Custs: { jsonCustResult } - ErrMsg: { jsonMarkResult }");
        //            }
        //        }
        //    }

        //    return paResultList;
        //}

        //public IEnumerable<ResultProblemAccountAddCustomerModel> MarkProblemAccountSiteLicensee(IEnumerable<ProblemAccountValidatedModel> models, int userID)
        //{
        //    var paResultList = new List<ResultProblemAccountAddCustomerModel>();

        //    if (models.Any())
        //    {
        //        var paCustList = GetProblemAccount(models);

        //        foreach (var cust in paCustList)
        //        {
        //            if (cust == null)
        //                continue;

        //            var markResult = MarkProblemAccountSiteLicensee(cust, userID);

        //            if (!string.IsNullOrEmpty(markResult.ErrMsg))
        //            {
        //                paResultList.Add(markResult);

        //                var jsonCustResult = JsonConvert.SerializeObject(cust);
        //                var jsonMarkResult = JsonConvert.SerializeObject(markResult);

        //                LogService.Error($"CTS - Mark Problem Account Site Licensee failed Custs: { jsonCustResult } - ErrMsg: { jsonMarkResult }");
        //            }
        //        }
        //    }

        //    return paResultList;
        //}

        //private ResultProblemAccountAddCustomerModel MarkProblemAccountSiteCredit(ProblemAccountAddCustomerModel model, int userID)
        //{
        //    var criteria = new ProblemAccountAddCustomerCriteria
        //    {
        //        CustomerList = model.CustomerList,
        //        RoleId = model.RoleId,
        //        ReasonId = model.ReasonId,
        //        PreventActionId = model.PreventActionId,
        //        Comment = model.Comment,
        //        UserID = userID,
        //        IP = string.Empty
        //    };

        //    using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
        //    {
        //        try
        //        {
        //            db.ExecuteNonQuery(criteria);                    

        //            return new ResultProblemAccountAddCustomerModel
        //            {
        //                ErrCode = criteria.ErrCode,
        //                ErrMsg = criteria.ErrMsg,
        //                SuccessList = criteria.SuccessList,
        //                FailList = criteria.FailList
        //            };
        //        }
        //        catch (Exception ex)
        //        {
        //            var jsonModel = JsonConvert.SerializeObject(model);

        //            LogService.Error($"CTS - Mark Problem Account Site Credit: { ex.Message } - data: { jsonModel }", ex);

        //            return new ResultProblemAccountAddCustomerModel
        //            {
        //                ErrCode = -1,
        //                ErrMsg = ex.Message
        //            };
        //        }
        //    }
        //}

        //private ResultProblemAccountAddCustomerModel MarkProblemAccountSiteLicensee(ProblemAccountAddCustomerModel model, int UserID)
        //{
        //    var criteria = new ProblemAccountLicenseeAddCustomerCriteria
        //    {
        //        CustomerList = model.CustomerList,
        //        ReasonId = model.ReasonId,
        //        ActionId = model.PreventActionId,
        //        Comment = model.Comment,
        //        UserID = UserID,
        //        IP = string.Empty
        //    };

        //    using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
        //    {
        //        try
        //        {
        //            db.ExecuteNonQuery(criteria);

        //            return new ResultProblemAccountAddCustomerModel
        //            {
        //                ErrCode = criteria.ErrCode,
        //                ErrMsg = criteria.ErrMsg
        //            };
        //        }
        //        catch (Exception ex)
        //        {
        //            var jsonModel = JsonConvert.SerializeObject(model);

        //            LogService.Error($"CTS - Mark Problem Account Site Licensee: { ex.Message } - data: { jsonModel }", ex);

        //            return new ResultProblemAccountAddCustomerModel
        //            {
        //                ErrCode = -1,
        //                ErrMsg = ex.Message
        //            };
        //        }
        //    }
        //}

        //public IEnumerable<ResultProblemAccountAddCustomerModel> UnmarkProblemAccountSiteCredit(IEnumerable<ProblemAccountValidatedModel> models, bool isExceptDownlinesByTVS, bool isExceptDownlinesByOPS, int userID)
        //{
        //    var paResultList = new List<ResultProblemAccountAddCustomerModel>();

        //    if (models.Any())
        //    {
        //        var paCustList = GetProblemAccount(models);

        //        foreach (var cust in paCustList)
        //        {
        //            if (cust == null)
        //                continue;

        //            var unmarkResult = UnmarkProblemAccountSiteCredit(cust, isExceptDownlinesByTVS, isExceptDownlinesByOPS, userID);

        //            if (!string.IsNullOrEmpty(unmarkResult.ErrMsg))
        //            {
        //                paResultList.Add(unmarkResult);

        //                var jsonCustResult = JsonConvert.SerializeObject(cust);
        //                var jsonMarkResult = JsonConvert.SerializeObject(unmarkResult);

        //                LogService.Error($"CTS - Unmark Problem Account Site Credit failed Custs: { jsonCustResult } - ErrMsg: { jsonMarkResult }");
        //            }
        //        }
        //    }

        //    return paResultList;
        //}

        //private ResultProblemAccountAddCustomerModel UnmarkProblemAccountSiteCredit(ProblemAccountAddCustomerModel model, bool isExceptDownlinesByTVS, bool isExceptDownlinesByOPS, int userID)
        //{
        //    var repository = new DynamicRepository();

        //    var criteria = new ProblemAccountRemoveCustomerCriteria
        //    {
        //        CustomerList = model.CustomerList,
        //        RoleID = model.RoleId,
        //        ExcludeAbnormalMember = isExceptDownlinesByTVS,
        //        ExcludeDirectMark = isExceptDownlinesByOPS,
        //        UserID = userID,
        //        IP = string.Empty
        //    };

        //    try
        //    {
        //        var resultUpdated = repository.Execute(criteria);

        //        if (string.IsNullOrEmpty(criteria.ErrMsg))
        //        {
        //            if (!string.IsNullOrEmpty(criteria.SuccessAllDownlineList))
        //            {
        //                var custs = criteria.SuccessAllDownlineList.Split(',').AsEnumerable();
        //                RemoveProblemAccounts(custs, userID);
        //            }
                    
        //        }                    

        //        return new ResultProblemAccountAddCustomerModel
        //        {
        //            ErrCode = criteria.ErrCode,
        //            ErrMsg = criteria.ErrMsg
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        var jsonModel = JsonConvert.SerializeObject(model);

        //        LogService.Error($"CTS - Unmark Problem Account Site Credit: { ex.Message } - data: { jsonModel }", ex);

        //        return new ResultProblemAccountAddCustomerModel
        //        {
        //            ErrCode = -1,
        //            ErrMsg = ex.Message
        //        };
        //    }
        //}

        //public IEnumerable<ResultProblemAccountAddCustomerModel> UnmarkProblemAccountSiteLicensee(IEnumerable<ProblemAccountValidatedModel> models, int userID)
        //{
        //    var paResultList = new List<ResultProblemAccountAddCustomerModel>();

        //    if (models.Any())
        //    {
        //        var paCustList = GetProblemAccount(models);

        //        foreach (var cust in paCustList)
        //        {
        //            if (cust == null)
        //                continue;

        //            var unmarkResult = UnmarkProblemAccountSiteLicensee(cust, userID);

        //            if (!string.IsNullOrEmpty(unmarkResult.ErrMsg))
        //            {
        //                paResultList.Add(unmarkResult);

        //                var jsonCustResult = JsonConvert.SerializeObject(cust);
        //                var jsonMarkResult = JsonConvert.SerializeObject(unmarkResult);

        //                LogService.Error($"CTS - Unmark Problem Account Site Licensee failed Custs: { jsonCustResult } - ErrMsg: { jsonMarkResult }");
        //            }
        //        }
        //    }

        //    return paResultList;
        //}

        //private ResultProblemAccountAddCustomerModel UnmarkProblemAccountSiteLicensee(ProblemAccountAddCustomerModel model, int userID)
        //{
        //    var repository = new DynamicRepository();

        //    var criteria = new ProblemAccountLicenseeRemoveCustomerCriteria
        //    {
        //        CustomerList = model.CustomerList,
        //        UserID = userID,
        //        IP = string.Empty
        //    };

        //    try
        //    {
        //        var resultUpdated = repository.Execute(criteria);

        //        if (string.IsNullOrEmpty(criteria.ErrMsg))
        //        {
        //            if (model.CustIds != null && model.CustIds.Any())
        //            {
        //                var custs = model.CustIds.Select(x => x.ToString());

        //                RemoveProblemAccounts(custs, userID);
        //            }
        //        }

        //        return new ResultProblemAccountAddCustomerModel
        //        {
        //            ErrCode = criteria.ErrCode,
        //            ErrMsg = criteria.ErrMsg
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        var jsonModel = JsonConvert.SerializeObject(model);

        //        LogService.Error($"CTS - Unmark Problem Account Site Licensee: { ex.Message } - data: { jsonModel }", ex);

        //        return new ResultProblemAccountAddCustomerModel
        //        {
        //            ErrCode = -1,
        //            ErrMsg = ex.Message
        //        };
        //    }
        //}

        private List<ProblemAccountAddCustomerModel> GetProblemAccount(IEnumerable<ProblemAccountValidatedModel> models)
        {
            var paCustList = new List<ProblemAccountAddCustomerModel>();
            paCustList.Add(GetProblemAccountAddCustomer(models, 4)); // Supper
            paCustList.Add(GetProblemAccountAddCustomer(models, 3)); // Master
            paCustList.Add(GetProblemAccountAddCustomer(models, 2)); // Agent
            paCustList.Add(GetProblemAccountAddCustomer(models, 1)); // Member

            return paCustList;
        }

        //private void UpdateCategories(IEnumerable<ResultProblemAccountUpdateModel> paUpdateds, int toReasonID, int userID)
        //{
        //    var apiEndPoint = ConfigSettings.UpdateCategories;

        //    var custInfos = paUpdateds.Select(x => new CustInfoModel
        //    {
        //        CustID = x.custId,
        //        FromReasonID = x.OldReasonId,
        //        ToReasonID = toReasonID
        //    });

        //    try
        //    {
        //        var paras = new UpdateCategoryParamsModel
        //        {
        //            CustInfos = custInfos,
        //            UserId = userID
        //        };

        //        var data = ApiHelper.Post<ApiResultModel>(apiEndPoint, paras);

        //        Task.WhenAll(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        var jsonModel = JsonConvert.SerializeObject(custInfos);

        //        LogService.Error($"CTS - Update Categories failed: { ex.Message } - data: {jsonModel}" + ex.Message, ex);
        //    }
        //}

        //private void RemoveProblemAccounts(IEnumerable<string> custIdList, int userID)
        //{
        //    try
        //    {
        //        var apiEndPoint = ConfigSettings.RemoveProblemAccounts;

        //        var paras = new RemovePANapParamsModel
        //        {
        //            CustIds = string.Join(",", custIdList),
        //            UserId = userID
        //        };

        //        var data = ApiHelper.Post<ApiResultModel>(apiEndPoint, paras);

        //        Task.WhenAll(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.Error($"CTS - Remove Problem Account failed: { ex.Message } - custIds: {string.Join(",", custIdList)}" + ex.Message, ex);
        //    }
        //}

        public byte[] ExportProblemAccount(IEnumerable<ProblemAccountViewModel> data, string fileName, string templatePath, ProblemAccountFilterViewModel filter)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "PAM");
            writer.CopyRows(0, 1);

            FilterBlock(filter, data.Count(), ref writer);
            writer.CopyRows(2, 1); // Fill Header

            FillRowData(data, ref writer);
            writer.SyncColumnsWidth(6);

            return writer.ToBytes();
        }

        private void FilterBlock(ProblemAccountFilterViewModel filter, int totalItems, ref ExcelTemplateWriter writer)
        {
            var boldFont = writer.GetFont(10, 0);
            var normalFont = writer.GetFont(11, 0);            

            var contentRender = new RichTextRender().Clear();
            AppendBlockContent(ref contentRender, "From", boldFont, filter.FromDate.ToString(Consts.FormatDateToString_MMMddyyyy), normalFont, "\n");
            WriteRickText(contentRender, ref writer, 4);

            contentRender = new RichTextRender().Clear();
            AppendBlockContent(ref contentRender, "To", boldFont, filter.ToDate.ToString(Consts.FormatDateToString_MMMddyyyy), normalFont, "\n");
            WriteRickText(contentRender, ref writer, 4);

            contentRender = new RichTextRender().Clear();
            AppendBlockContent(ref contentRender, "Site", boldFont, filter.IsAllSites ? "All" : filter.Sites, normalFont, "\n");
            WriteRickText(contentRender, ref writer, 4);

            contentRender = new RichTextRender().Clear();
            AppendBlockContent(ref contentRender, "Username", boldFont, " ", normalFont, "\n");
            WriteRickText(contentRender, ref writer, 4);

            contentRender = new RichTextRender().Clear();
            AppendBlockContent(ref contentRender, "Search downlines of this account", boldFont, filter.HasDownline ? "yes" : " ", normalFont, "\n");
            WriteRickText(contentRender, ref writer, 4);

            contentRender = new RichTextRender().Clear();
            AppendBlockContent(ref contentRender, "Total items", boldFont, totalItems.ToString(), normalFont, "\n");
            WriteRickText(contentRender, ref writer, 4);

            var dateTime = filter.DateTime.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss2);
            contentRender = new RichTextRender().Clear();
            AppendBlockContent(ref contentRender, "Export date time", boldFont, dateTime, normalFont, "\n");             
            WriteRickText(contentRender, ref writer, 4);          
        }

        private void WriteRickText(RichTextRender contentRender, ref ExcelTemplateWriter writer, int idx)
        {
            var contentRichTextString = contentRender.XSSFRickRender();
            XSSFRichTextString[] contentRichTextArray = new XSSFRichTextString[1];
            contentRichTextArray[0] = contentRichTextString;
            writer.WriteRow(idx, contentRichTextArray);
        }

        private void FillRowData(IEnumerable<ProblemAccountViewModel> problemAccounts, ref ExcelTemplateWriter writer)
        {
            foreach (var item in problemAccounts)
            {
                var record = new object[]
                {
                     item.UserName,
                     item.Site,
                     item.Currency,
                     item.Category,
                     item.CreatedBy,
                     item.CreatedDateStr
                };

                writer.WriteRow(3, record);
            }
        }

        private void AppendBlockContent(ref RichTextRender content, string name, IFont nameFont, string value, IFont valueFont, string endString)
        {
            TextBlock nameBlock = new TextBlock(name + ": ", nameFont);
            content = content.AddText(nameBlock);

            TextBlock valueBlock = new TextBlock(value, valueFont);
            content = content.AddText(valueBlock);

            if (!string.IsNullOrEmpty(endString))
            {
                content = content.AddText(endString);
            }            
        }      
       
		//public IEnumerable<long> UpdateProblemAccountEvidence(UpdateCategoryParamsModel paras)
  //      {
  //          var custIdsJson = JsonConvert.SerializeObject(paras.CustInfos);
  //          var repository = new DynamicRepository();

  //          var criteria = new UpdateProblemAccountEvidenceCriteria
  //          {
  //              CustInfo = custIdsJson,
  //              UserID = paras.UserId,
  //              SourceType = 1
  //          };

  //          LogService.Info($"CTS Portal API - UpdateProblemAccountEvidence custIdsJson:  {custIdsJson}");

  //          return repository.Fetch<long>(criteria);
  //      }

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
