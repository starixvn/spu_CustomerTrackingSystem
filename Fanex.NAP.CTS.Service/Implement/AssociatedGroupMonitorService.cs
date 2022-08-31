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
    using Fanex.NAP.CTS.Model.Account.CustomerInfo;
    using Fanex.NAP.CTS.Model.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using Newtonsoft.Json;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IAssociatedGroupMonitorService
    {        
        IEnumerable<AssociatedGroupMonitorModel> GetAssociatedGroupMonitor();

        IEnumerable<AssociatedAccountMonitorModel> GetAssociatedAccountMonitor(long groupID);

        DBResult AddAssociatedGroupMonitor(AddAssociatedGroupViewModel model);

        DBResult RemoveAssociatedGroupMonitor(RemoveAssociatedGroupViewModel model);

        IEnumerable<ValidateUserAssociatedAccountModel> ValidateUserNameInput(string userNames);

        DBResult AddAssociatedAccount(AddAssociatedAccountViewModel model);

        DBResult UpdateABIToMainDB(string custIDs, int valueABI);

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

        public IEnumerable<ValidateUserAssociatedAccountModel> ValidateUserNameInput(string userNames)
        {
            var criteria = new ValidateAssociatedAccountCriteria
            {
                UserNames = userNames
            };

            return repository.Fetch<ValidateUserAssociatedAccountModel>(criteria);
        }

        public DBResult AddAssociatedAccount(AddAssociatedAccountViewModel model)
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
                repository.Execute(criteria);

                return new DBResult { ErrorCode = 0 };
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Add associated account Exception { logMessage }", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public DBResult UpdateABIToMainDB(string custIDs, int valueABI)
        {
            var custInfos = GetCustomerInfoMainDB(custIDs, 2);

            try
            {
                var custToUpdateABI = custInfos.Where(x => x.Danger2 == 0).Select(x => new DangerLevelModel
                {
                    CustId = x.CustID,
                    DangerLevel = valueABI
                });

                if (custToUpdateABI.Any())
                {
                    return customerClassificationService.UpdateDangerLevelToMainDB(custToUpdateABI, 2);
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

        public DBResult AddAssociatedGroupMonitor(AddAssociatedGroupViewModel model)
        {
            var criteria = new AddAssociatedGroupMonitorCriteria
            {
                GroupName = model.GroupName,
                ABI = model.ABI,
                Remark = model.Comment,
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

        public DBResult RemoveAssociatedGroupMonitor(RemoveAssociatedGroupViewModel model)
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

            var headerRow = writer.CopyRows(0, 1);
            var headerCell = headerRow.GetCell(0);

            headerCell.SetCellValue(model.GroupName.ToUpper() + " GROUP MANAGEMENT REPORT");

            writer.CopyRows(1, 1);
            writer.CopyRows(2, 1);

            FillRowData(associatedAccounts, ref writer);

            writer.SyncColumnsWidth(11);

            return writer.ToBytes();
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
