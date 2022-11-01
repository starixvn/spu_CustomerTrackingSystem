namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.Account.CustomerInfo;
    using Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.Criteria.Common;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account.CustomerInfo;
    using Fanex.NAP.CTS.Model.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using Fanex.NAP.TGRS.Core.ExcelUtility.RichTextExtensions;
    using Newtonsoft.Json;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IAssociatedGroupMonitorService
    {        
        IEnumerable<AssociatedGroupMonitorModel> GetAssociatedGroupMonitor();

        IEnumerable<AssociatedGroupSiteModel> GetAssociatedGroupSites(long groupID);

        IEnumerable<ProblemCategorySiteModel> GetProblemCategorySites(long groupID);

        IEnumerable<AssociatedAccountMonitorModel> GetAssociatedAccountMonitor(long groupID);

        DBResult AddAssociatedGroupMonitor(AddAssociatedGroupModel model);

        GetUpdateAssociatedGroupMonitorByIDModel GetUpdateAssociatedGroupMonitorByID(long groupID);

        DBResult UpdateAssociatedGroupMonitor(UpdateAssociatedGroupMonitorModel model);

        DBResult RemoveAssociatedGroupMonitor(RemoveAssociatedGroupModel model);

        IEnumerable<ValidateUserAssociatedAccountModel> ValidateUserNameInput(string userNames);

        ResultAddAssociatedAccountModel AddAssociatedAccount(AddAssociatedAccountModel model);

        GetProblemCategoryByGroupIDModel GetProblemCategoryByGroupID(long groupID);

        DBResult SetProblemCategoryByGroupID(SetProblemCategoryByGroupIDModel model);

        DBResult UpdateABIToMainDB(string custIDs, int valueABI);

        DBResult UpdateOriToMainDB(string custIDs, int valueOri);

        byte[] ExportExcelAssociatedAccount(IEnumerable<AssociatedAccountMonitorModel> associatedAccounts
            , string fileName
            , string templatePath
            , ExportToExcelAssociatedAccountViewModel model);

    }

    public class AssociatedGroupMonitorService : IAssociatedGroupMonitorService
    {
        private readonly IDynamicRepository repository;
        private readonly ICustomerClassificationService customerClassificationService;

        public AssociatedGroupMonitorService()
        {
            repository = new DynamicRepository();
            customerClassificationService = new CustomerClassificationService();
        }

        public IEnumerable<SiteModel> GetSites()
        {
            var criteria = new GetSitesCriteria();

            return repository.Fetch<SiteModel>(criteria);
        }

        public IEnumerable<AssociatedGroupMonitorModel> GetAssociatedGroupMonitor()
        {
            var criteria = new GetAssociatedGroupMonitorCriteria();

            return repository.Fetch<AssociatedGroupMonitorModel>(criteria);
        }

        public IEnumerable<AssociatedAccountMonitorModel> GetAssociatedAccountMonitor(long groupID)
        {
            var criteria = new GetAssociatedAccountCriteria
            {
                GroupID = groupID
            };

            return repository.Fetch<AssociatedAccountMonitorModel>(criteria);
        }

        public IEnumerable<AssociatedGroupSiteModel> GetAssociatedGroupSites(long groupID)
        {
            var criteria = new GetAssociatedGroupSiteCriteria { GroupID = groupID };

            return repository.Fetch<AssociatedGroupSiteModel>(criteria);
        }

        public IEnumerable<ProblemCategorySiteModel> GetProblemCategorySites(long groupID)
        {
            var criteria = new GetProblemCategorySitesCriteria { GroupID = groupID };

            return repository.Fetch<ProblemCategorySiteModel>(criteria);
        }

        public IEnumerable<ValidateUserAssociatedAccountModel> ValidateUserNameInput(string userNames)
        {
            var criteria = new ValidateAssociatedAccountCriteria
            {
                UserNames = userNames
            };

            return repository.Fetch<ValidateUserAssociatedAccountModel>(criteria);
        }

        public ResultAddAssociatedAccountModel AddAssociatedAccount(AddAssociatedAccountModel model)
        {
            var criteria = new AddAssociatedAccountCriteria
            {
                GroupID = model.GroupID,
                CTSCustIDs = model.CTSCustIDs,
                IsAuto = model.IsAuto,
                Remark = model.Comment,
                CreatedBy = model.CreatedBy
            };

            try
            {              
                var custInfos = repository.Fetch<CustInfoAssociatedAccountModel>(criteria);

                return new ResultAddAssociatedAccountModel {  ErrorCode = 0, CustInfos = custInfos };
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Add associated account Exception { logMessage }", ex);
                
                return new ResultAddAssociatedAccountModel { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public GetProblemCategoryByGroupIDModel GetProblemCategoryByGroupID(long groupID)
        {
            var criteria = new GetProblemCategoryByGroupIDCriteria
            {
                GroupID = groupID
            };

            return repository.Fetch<GetProblemCategoryByGroupIDModel>(criteria).FirstOrDefault();
        }

        public DBResult SetProblemCategoryByGroupID(SetProblemCategoryByGroupIDModel model)
        {
            var criteria = new SetProblemCategoryByGroupIDCriteria
            {
                GroupID = model.GroupID,
                PACategoryID = model.PACategoryID,
                PAAllCredit = model.PAAllCredit,
                PAAllLicensee = model.PAAllLicensee,
                PASites = model.PASites,
                CreatedBy = model.CreatedBy
            };

            try
            {
                repository.Execute(criteria);

                return new DBResult { ErrorCode = 0 };
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Set ProblemCategoryByGroupID Exception { logMessage }", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public DBResult UpdateABIToMainDB(string custIDs, int valueABI)
        {
            var custInfos = GetCustomerInfoMainDB(custIDs, 2);

            try
            {
               
                if (valueABI > 0)
                {
                    var custToUpdateABI = custInfos.Where(x => x.Danger2 == 0).Select(x => new DangerLevelModel
                    {
                        CustId = x.CustID,
                        DangerLevel = valueABI
                    });

                    if (custToUpdateABI.Any())
                    {
                        customerClassificationService.UpdateDangerLevelToMainDB(custToUpdateABI, 2);
                    }
                }                
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                var jsonData = JsonConvert.SerializeObject(custInfos);

                LogService.Error($"Update ABI To MainDB Exception { logMessage } with listCust: {jsonData}", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }            

            return new DBResult { ErrorCode = 0 };
        }

        public DBResult UpdateOriToMainDB(string custIDs, int valueOri)
        {
            var custInfos = GetCustomerInfoMainDB(custIDs, 2);

            try
            {
                if (valueOri > 0)
                {
                    var custToUpdateOri = custInfos.Where(x => x.Danger1 == 0).Select(x => new DangerLevelModel
                    {
                        CustId = x.CustID,
                        DangerLevel = valueOri
                    });

                    if (custToUpdateOri.Any())
                    {
                        customerClassificationService.UpdateDangerLevelToMainDB(custToUpdateOri, 1);
                    }
                }

            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                var jsonData = JsonConvert.SerializeObject(custInfos);

                LogService.Error($"Update Ori To MainDB Exception { logMessage } with listCust: {jsonData}", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };              
            }

            return new DBResult { ErrorCode = 0 };
        }

        public IEnumerable<CustomerInfoByCustIdsModel> GetCustomerInfoMainDB(string custIdList, int sourceType)
        {
            try
            {
                var criteria = new GetCustomerInfoByCustIdsCriteria
                {
                    CustIdList = custIdList,
                    SourceType = sourceType
                };

                return repository.Fetch<CustomerInfoByCustIdsModel>(criteria);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Get ABI from MainDB Exception { logMessage } custIdList: {custIdList } sourceType: {sourceType}", ex);
            }

            return Enumerable.Empty<CustomerInfoByCustIdsModel>();
        }       

        public DBResult AddAssociatedGroupMonitor(AddAssociatedGroupModel model)
        {
            var assTypes = model.AssociationTypes.Split(',').Select(int.Parse);

            var criteria = new AddAssociatedGroupMonitorCriteria
            {
                GroupName = model.GroupName,
                ABI = model.ABI,
                Ori = model.Ori,
                AllCredit = model.AllCredit,
                AllLicensee = model.AllLicensee,
                Sites = model.Sites,
                HasDevice = assTypes.Any(x => x == (int)AssociationType.Device),
                HasManual = assTypes.Any(x => x == (int)AssociationType.Manual),
                HasIP = assTypes.Any(x => x == (int)AssociationType.IP),
                HasAI = assTypes.Any(x => x == (int)AssociationType.BettingPattern),
                Remark = model.Remark,
                CreatedBy = model.CreatedBy
            };

            try
            {
                repository.Execute(criteria);

                return new DBResult { ErrorCode = 0 };
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Add associated group monitor Exception { logMessage }", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public GetUpdateAssociatedGroupMonitorByIDModel GetUpdateAssociatedGroupMonitorByID(long groupID)
        {
            var criteria = new GetAssociatedGroupMonitorByIDCriteria
            {
                GroupID = groupID
            };

            return repository.Fetch<GetUpdateAssociatedGroupMonitorByIDModel>(criteria).First() ?? new GetUpdateAssociatedGroupMonitorByIDModel();
        }

        public DBResult UpdateAssociatedGroupMonitor(UpdateAssociatedGroupMonitorModel model)
        {
            var assTypes = model.AssociationTypes.Split(',').Select(int.Parse);

            var criteria = new UpdateAssociatedGroupMonitorCriteria
            {
                GroupID = model.GroupID,
                GroupName = model.GroupName,
                ABI = model.ABI,
                Ori = model.Ori,
                AllCredit = model.AllCredit,
                AllLicensee = model.AllLicensee,
                Sites = model.Sites,
                HasDevice = assTypes.Any(x => x == (int)AssociationType.Device),
                HasManual = assTypes.Any(x => x == (int)AssociationType.Manual),
                HasIP = assTypes.Any(x => x == (int)AssociationType.IP),
                HasAI = assTypes.Any(x => x == (int)AssociationType.BettingPattern),
                Remark = model.Remark,
                ModifiedBy = model.ModifiedBy
            };

            try
            {
                repository.Execute(criteria);

                return new DBResult { ErrorCode = 0 };
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Update associated group monitor Exception { logMessage }", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public DBResult RemoveAssociatedGroupMonitor(RemoveAssociatedGroupModel model)
        {
            var criteria = new RemoveAssociatedGroupMonitorCriteria
            {
                GroupID = model.GroupID,
                UserID = model.UserID
            };

            try
            {
                repository.Execute(criteria);

                return new DBResult { ErrorCode = 0 };
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Remove associated group monitor Exception { logMessage }", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public byte[] ExportExcelAssociatedAccount(IEnumerable<AssociatedAccountMonitorModel> associatedAccounts
            , string fileName
            , string templatePath
            , ExportToExcelAssociatedAccountViewModel model)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "GMR");
            writer.CopyRows(0, 1);
            
            FilterBlock(model, ref writer);

            writer.CopyRows(2, 1);

            FillRowData(associatedAccounts, ref writer);

            writer.SyncColumnsWidth(11);

            return writer.ToBytes();
        }

        private void FilterBlock(ExportToExcelAssociatedAccountViewModel model, ref ExcelTemplateWriter writer)
        {
            var boldFont = writer.GetFont(10, 0);
            var normalFont = writer.GetFont(11, 0);
            var lineCount = 1;

            var contentRender = new RichTextRender().Clear();
            if (!string.IsNullOrEmpty(model.GroupName))
                AppendBlockContent(ref contentRender, ref lineCount, "Group Name", boldFont, model.GroupName, normalFont, "\n");

            if (model.IsAllSite)
            {
                AppendBlockContent(ref contentRender, ref lineCount, "Site", boldFont, "All", normalFont, "\n");
            }
            else
            {
                AppendBlockContent(ref contentRender, ref lineCount, "Site", boldFont, " ", normalFont, "\n");

                if (!string.IsNullOrEmpty(model.CreditSites))
                    AppendBlockContent(ref contentRender, ref lineCount, "- Credit", boldFont, model.CreditSites, normalFont, "\n");

                if (!string.IsNullOrEmpty(model.LicenseeSites))
                    AppendBlockContent(ref contentRender, ref lineCount, "- Licensee", boldFont, model.LicenseeSites, normalFont, "\n");
            }           

            if (!string.IsNullOrEmpty(model.AssociatedType))
                AppendBlockContent(ref contentRender, ref lineCount, "Associated Type", boldFont, model.AssociatedType, normalFont, "\n");

            WriteRickText(contentRender, ref writer, 4);

            writer.SetRowHeight(1, 225 * lineCount);
        }

        private void WriteRickText(RichTextRender contentRender, ref ExcelTemplateWriter writer, int idx)
        {
            var contentRichTextString = contentRender.XSSFRickRender();
            XSSFRichTextString[] contentRichTextArray = new XSSFRichTextString[1];
            contentRichTextArray[0] = contentRichTextString;
            writer.WriteRow(idx, contentRichTextArray);
        }

        private void AppendBlockContent(ref RichTextRender content, ref int lineCount, string name, IFont nameFont, string value, IFont valueFont, string endString)
        {
            TextBlock nameBlock = new TextBlock(name + ": ", nameFont);
            content = content.AddText(nameBlock);

            TextBlock valueBlock = new TextBlock(value, valueFont);
            content = content.AddText(valueBlock);

            if (!string.IsNullOrEmpty(endString))
            {
                content = content.AddText(endString);
            }

            lineCount += 1;
        }

        private void FillRowData(IEnumerable<AssociatedAccountMonitorModel> associatedAccounts, ref ExcelTemplateWriter writer)
        {
            foreach (var item in associatedAccounts)
            {
                var record = new object[]
                {
                    item.Username,
                    item.AccountStatus,
                    item.AccountCreated,
                    item.Site,
                    item.Currency,
                    item.ABI,
                    item.Ori,
                    item.Category,
                    item.Remark,                    
                    item.DetectedDate,
                    item.DetectedBy
                };

                writer.WriteRow(3, record);
            }
        }

    }
}
